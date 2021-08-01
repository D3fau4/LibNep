using System;
using System.Collections.Generic;
using LibNep.Images;
using System.Drawing;
using System.Drawing.Imaging;
using Yarhl.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace LibNep.FileFormats
{
    public class TID
    {
        static byte[] magic = { 0x54, 0x49, 0x44 };
        DataReader reader;
        Bitmap image;
        public byte version = 0x0;
        public string Name = "";
        public int Width = 0;
        public int Height = 0;
        public int DataLength = 0;
        public CompressionType CompressionType;

        public TID(string path)
        {
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            load();
        }

        public TID(Stream file)
        {
            var stream = DataStreamFactory.FromStream(file);
            reader = new DataReader(stream);
            load();
        }
        public TID(DataStream stream)
        {
            reader = new DataReader(stream);
            load();
        }

        internal void load()
        {
            var origin = reader.Stream.Position;
            // Read Magic
            var _magic = reader.ReadBytes(3);
            if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2])
                throw new Exception("Sistema de archivos no reconocido");

            // Read Version
            version = reader.ReadByte();
            if ((version & 0x01) == 0)
                reader.Endianness = EndiannessMode.LittleEndian;
            else
                reader.Endianness = EndiannessMode.BigEndian;

            // Read Name
            reader.Stream.Seek(0x20 + origin, SeekMode.Start);
            Name = reader.ReadString();

            // Read Sizes
            reader.Stream.Seek(0x44 + origin, SeekMode.Start);
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            // Data lenght
            reader.Stream.Seek(0x58 + origin, SeekMode.Start);
            DataLength = reader.ReadInt32();

            // Compression type
            reader.Stream.Seek(0x64 + origin, SeekMode.Start);
            CompressionType = (CompressionType)(byte)reader.ReadInt32();

            if (reader.Stream.Length < 0x80 + DataLength)
                throw new EndOfStreamException();
            
            reader.Stream.Seek(0x80 + origin, SeekMode.Start);
            byte[] data;
            if (version  == 0x9A)
                data = reader.ReadBytes(Width * Height * 4);
            else 
                data = reader.ReadBytes(DataLength);

            if (CompressionType == CompressionType.None){
                if (version == 0x9A)
                    throw new NotImplementedException();
                else if ((version & 0x02) == 0x02)
                    data = BitmapArrayTools.Swap32BppColorChannels(data, 3, 2, 1, 0);
                else
                    data = BitmapArrayTools.Swap32BppColorChannels(data, 2, 1, 0, 3);
            }

            var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            var bitmapdata = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(data, 0, bitmapdata.Scan0, data.Length);
            bitmap.UnlockBits(bitmapdata);

            image = bitmap;
        }
    }
}
