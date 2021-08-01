using System;
using System.Collections.Generic;
using Yarhl.IO;
using System.IO;

namespace LibNep.FileFormats
{
    public class TID
    {
        static byte[] magic = { 0x54, 0x49, 0x44 };
        DataReader reader;
        public byte version = 0x0;
        public string Name = "";
        public int Width = 0;
        public int Height = 0;
        public int DataLength = 0;

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

            // Read Name
            reader.Stream.Seek(0x20 + origin, SeekMode.Start);
            Name = reader.ReadString();

            // Read Sizes
            reader.Stream.Seek(0x44 + origin, SeekMode.Start);
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            reader.Stream.Seek(0x54 + origin, SeekMode.Start);
            DataLength = reader.ReadInt32();
        }
    }
}
