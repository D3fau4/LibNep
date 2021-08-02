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
        int version;
        int PartsCount;
        int FPS;
        PART[] parts;

        public SSA(string path)
        {
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            load();
        }
        public SSA(Stream stream)
        {
            var stream2 = DataStreamFactory.FromStream(stream);
            reader = new DataReader(stream2);
            load();
        }
        public SSA(DataStream stream)
        {
            reader = new DataReader(stream);
            load();
        }

        internal void load()
        {
            var origin = reader.Stream.Position;

            // Read Magic
            var _magic = reader.ReadBytes(4);
            if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2] || magic[3] != _magic[3])
                throw new Exception("Sistema de archivos no reconocido");

            reader.Stream.Seek(0x4 + origin, SeekMode.Start);
            version = reader.ReadInt32();

            // Read the number of entrys/parts
            reader.Stream.Seek(0x14 + origin);
            PartsCount = reader.ReadInt32();
            //parts = new PART[PartsCount]();
            
            // Frames per second (?)
            reader.Stream.Seek(0x18 + origin);
            FPS = reader.ReadInt32();
            
            // UNK bytes
            reader.Stream.Seek(0x1C + origin);
            var Unk1 = reader.ReadInt32();

            // Read parts
            PART[] parts = PART.GetPARTs(reader, PartsCount);
        }
    }
}
