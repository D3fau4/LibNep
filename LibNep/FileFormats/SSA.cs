using System;
using System.IO;
using Yarhl.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNep.FileFormats.SSAD;

namespace LibNep.FileFormats
{
    public class SSA
    {
        static byte[] magic = { 0x53, 0x53, 0x41, 0x44 };
        DataReader reader;
        DataWriter writer;
        int version;
        int PartsCount;
        int FPS;
        Stream stream;
        PART[] parts;
        string filepath;

        public SSA(string path)
        {
            filepath = path;
            stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            reader.DefaultEncoding = Encoding.GetEncoding(932);
        }
        public SSA(Stream stream)
        {
            var stream2 = DataStreamFactory.FromStream(stream);
            reader = new DataReader(stream2);
            reader.DefaultEncoding = Encoding.GetEncoding(932);
        }
        public SSA(DataStream stream)
        {
            reader = new DataReader(stream);
            reader.DefaultEncoding = Encoding.GetEncoding(932);
        }

        public void load()
        {
            var origin = reader.Stream.Position;

            // Read Magic
            var _magic = reader.ReadBytes(4);
            if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2] || magic[3] != _magic[3])
                throw new Exception("Sistema de archivos no reconocido");

            version = reader.ReadInt32();

            // Read the number of entrys/parts
            reader.SkipPadding(0x14);
            PartsCount = reader.ReadInt32();
            
            // Frames per second (?)
            FPS = reader.ReadInt32();
            
            // Scale bytes
            var Unk1 = reader.ReadInt32();

            // Read parts
            List<PART> parts = new PART().GetPARTs(reader, PartsCount);
        }

        public Stream Upscale(int upsize)
        {
            reader.Stream.Seek(0, SeekOrigin.Begin);
            var stream = DataStreamFactory.FromMemory();
            writer = new DataWriter(stream);
            new PART().Upscale(reader, writer, upsize);
            
            return writer.Stream;
        }

        public void Close()
        {
            reader.Stream.Close();
            writer.Stream.Close();
            stream.Close();
        }
    }
}
