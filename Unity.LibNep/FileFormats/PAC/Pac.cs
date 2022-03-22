using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unity.LibNep.Utils;
using Yarhl.IO;

namespace LibNep.FileFormats.PAC
{
    public static class EncodingCache
    {
        public static readonly Encoding ShiftJIS;

        static EncodingCache()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ShiftJIS = Encoding.GetEncoding(932);
        }
    }

    public class Pac
    {
        DataReader reader;
        DataWriter writer;

        private Stream mBaseStream;

        public int Field08 { get; set; }
        public int Index { get; set; }
        public List<PacFileEntry> Entries { get; }

        public Pac()
        {
            Entries = new List<PacFileEntry>();
        }

        public Pac(string path)
        {
            Entries = new List<PacFileEntry>();
            mBaseStream = File.OpenRead(path);
            Read(mBaseStream);
        }

        public Pac(Stream stream)
        {
            Entries = new List<PacFileEntry>();
            mBaseStream = stream;
            Read(stream);
        }

        public void Read(Stream stream)
        {
            Entries.Clear();

            var reader = new DataReader(stream);
            var header = reader.Read<DwPackHeader>();
            Field08 = header.Field08;
            Index = header.Index;

            var dataStartOffset = DwPackHeader.SIZE + (header.FileCount * PacEntry.SIZE);
            for (int i = 0; i < header.FileCount; i++)
            {
                var entry = reader.Read<PacEntry>();
                Entries.Add(new PacFileEntry(stream, dataStartOffset, entry));
            }
        }

        public void Unpack(string directoryPath, Func<PacFileEntry, bool> callback)
        {
            var baseStreamLock = new object();
            Parallel.ForEach(Entries, (entry =>
            {
                if (callback != null && !callback(entry)) return;
                var unpackPath = Path.Combine(directoryPath, entry.Path);
                var unpackDir = Path.GetDirectoryName(unpackPath);
                Directory.CreateDirectory(unpackDir);

                using var fileStream = File.Create(unpackPath);
                if (entry.IsCompressed)
                {
                    // Copy compressed data from basestream so we can decompress in parallel
                    var compressedBufferMem = MemoryPool<byte>.Shared.Rent(entry.CompressedSize);
                    var compressedBuffer = compressedBufferMem.Memory.Span.Slice(0, entry.CompressedSize);
                    lock (baseStreamLock)
                        entry.CopyTo(compressedBuffer, decompress: false);

                    // Decompress
                    using var outBufferMem = MemoryPool<byte>.Shared.Rent(entry.UncompressedSize);
                    var outBuffer = outBufferMem.Memory.Span.Slice(0, entry.UncompressedSize);
                    HuffmanCodec.Decompress(compressedBuffer.ToArray(), outBuffer.ToArray(), (uint)entry.CompressedSize, (uint)entry.UncompressedSize);
                    fileStream.Write(outBuffer);
                }
                else
                {
                    lock (baseStreamLock)
                        entry.CopyTo(fileStream, decompress: false);
                }
            }));
        }

        public void Write(Stream stream, bool compress, Action<PacFileEntry> callback)
        {
            var writer = new DataWriter(stream);

            DwPackHeader header;
            header.Signature = DwPackHeader.SIGNATURE;
            header.Field08 = Field08;
            header.FileCount = Entries.Count;
            header.Index = Index;
            // Cacas
            writer.Write(DwPackHeader.SIGNATURE);
            writer.Write(Field08);
            writer.Write(Entries.Count);
            writer.Write(Index);

            var dataOffset = 0;
            for (int i = 0; i < Entries.Count; i++)
            {
                var e = Entries[i];
                /*if (compress)
                    e.Compress();*/

                var entry = new PacEntry();
                entry.Field00 = e.Field00;
                entry.Index = (short)i;
                entry.PackIndex = (short)header.Index;
                entry.Path = e.Path;
                entry.Field104 = e.Field104;
                entry.CompressedSize = e.CompressedSize;
                entry.UncompressedSize = e.UncompressedSize;
                entry.Flags = e.IsCompressed ? 1 : 0;
                entry.DataOffset = dataOffset;
                writer.Write(entry.Field00);
                writer.Write(entry.Index);
                writer.Write(entry.PackIndex);
                writer.Write(entry.Path);
                writer.Write(entry.Field104);
                writer.Write(entry.CompressedSize);
                writer.Write(entry.UncompressedSize);
                writer.Write(entry.Flags);
                writer.Write(entry.DataOffset);
                dataOffset += entry.CompressedSize;
            }

            for (int i = 0; i < Entries.Count; i++)
            {
                callback?.Invoke(Entries[i]);
                Entries[i].CopyTo(writer.Stream, decompress: false);
            }
        }

        public void AddFiles(string directoryPath, bool compress, Func<string, bool> callback)
        {
            Parallel.ForEach(Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories), (path =>
            {
                if (callback != null && !callback(path)) return;
                var relativePath = path.Substring(path.IndexOf(directoryPath) + directoryPath.Length + 1);
                Entries.Add(new PacFileEntry(relativePath, File.OpenRead(path), compress));
            }));
        }

        public static Pac Pack(string directoryPath, int index, bool compress, Func<string, bool> callback)
        {
            var pack = new Pac();
            pack.Index = index;
            pack.AddFiles(directoryPath, compress, callback);
            return pack;
        }
    }

    public class PacFileEntry
    {
        private Stream mBaseStream;
        private long mDataOffset;
        private Stream mDataStream;

        public int Field00 { get; set; }
        public string Path { get; set; }
        public int Field104 { get; set; }
        public int CompressedSize { get; private set; }
        public int UncompressedSize { get; private set; }
        public bool IsCompressed { get; private set; }

        public PacFileEntry(string path, Stream stream, bool compress)
        {
            Field00 = 0;
            Path = path;
            mDataStream = stream;
            CompressedSize = UncompressedSize = (int)stream.Length;
            IsCompressed = false;

            if (compress)
                Compress();

            
        }

        public PacFileEntry(Stream baseStream, long dataStartOffset, PacEntry entry)
        {
            mBaseStream = baseStream;
            mDataOffset = dataStartOffset + entry.DataOffset;
            Field00 = entry.Field00;
            Path = entry.Path;
            Field104 = entry.Field104;
            CompressedSize = entry.CompressedSize;
            UncompressedSize = entry.UncompressedSize;
            IsCompressed = entry.Flags > 0;
        }

        public Stream Open(bool decompress)
        {
            if (mDataStream == null)
            {
                DataReader reader = new DataReader(mBaseStream);
                reader.Stream.Seek(mDataOffset, SeekOrigin.Begin);
                mDataStream = new MemoryStream(reader.ReadBytes(CompressedSize));
                if (decompress)
                    Decompress();
            }
            DataReader reader1 = new DataReader(mDataStream);
            reader1.Stream.Seek(0, SeekOrigin.Begin);
            return new MemoryStream(reader1.ReadBytes(CompressedSize)); //return mDataStream.Slice(0, mDataStream.Length);
        }

        private void Decompress()
        {
            if (IsCompressed)
            {
                mDataStream = new MemoryStream(UncompressedSize);
                CopyTo(mDataStream, decompress: true);
                IsCompressed = false;
                CompressedSize = UncompressedSize;
            }
        }

        private void Compress()
        {
            if (!IsCompressed)
            {
                using var inBufferMem = MemoryPool<byte>.Shared.Rent(CompressedSize);
                var inBuffer = inBufferMem.Memory.Span.Slice(0, CompressedSize);
                mDataStream.Position = 0;
                mDataStream.Read(inBufferMem.Memory.Span.Slice(0, CompressedSize));

                using var outBufferMem = MemoryPool<byte>.Shared.Rent(UncompressedSize);
                var outBuffer = outBufferMem.Memory.Span.Slice(0, UncompressedSize);
                int compressedSize = 0;
                if (UncompressedSize > 0)
                    compressedSize = HuffmanCodec.Compress(inBuffer.ToArray(), outBuffer.ToArray(), compressedSize);

                mDataStream = new MemoryStream(UncompressedSize);
                mDataStream.Write(outBuffer.Slice(0, compressedSize));

                IsCompressed = true;
                CompressedSize = compressedSize;
            }
        }

        public void CopyTo(Stream destination, bool decompress)
        {
            if (IsCompressed && decompress)
            {
                using var inBufferMem = MemoryPool<byte>.Shared.Rent(CompressedSize);
                var inBuffer = inBufferMem.Memory.Span.Slice(0, CompressedSize);
                Open(false).Read(inBufferMem.Memory.Span.Slice(0, CompressedSize));

                using var outBufferMem = MemoryPool<byte>.Shared.Rent(UncompressedSize);
                var outBuffer = outBufferMem.Memory.Span.Slice(0, UncompressedSize);
                HuffmanCodec.Decompress(inBuffer.ToArray(), outBuffer.ToArray(), (uint)CompressedSize, (uint)UncompressedSize);
                destination.Write(outBuffer);
            }
            else
            {
                Open(false).CopyTo(destination);
            }
        }

        public void CopyTo(Span<byte> destination, bool decompress)
        {
            if (IsCompressed && decompress)
            {
                using var inBufferMem = MemoryPool<byte>.Shared.Rent(CompressedSize);
                var inBuffer = inBufferMem.Memory.Span.Slice(0, CompressedSize);
                Open(false).Read(inBufferMem.Memory.Span.Slice(0, CompressedSize));
                //HuffmanCodec.Decompress(inBuffer, destination);
            }
            else
            {
                Open(false).Read(destination);
            }
        }
    }
}
