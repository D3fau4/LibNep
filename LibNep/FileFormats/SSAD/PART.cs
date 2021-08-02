using System;
using System.IO;
using Yarhl.IO;

namespace LibNep.FileFormats.SSAD
{
    public class PART
    {
        static public byte[] magic = { 0x50, 0x41, 0x52, 0x54 };
        public int Version = 0;
        public int Index = 0;
        public string Name = "";
        public int Sprite_positionX = 0;
        public int Sprite_positionY = 0;

        public static PART[] GetPARTs(DataReader reader, int numberofentrys)
        {
            PART[] parts = new PART[numberofentrys];
            reader.Stream.Seek(0x20, SeekMode.Start);
            var origin = reader.Stream.Position;
            for (int i = 0; i < numberofentrys; i++)
            {
                reader.Stream.Seek(0x20, SeekMode.Start);
                parts[i] = new PART();
                // First PART
                var _magic = reader.ReadBytes(4);
                if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2] || magic[3] != _magic[3])
                    throw new Exception("Incorrect file format");

                parts[i].Version = reader.ReadInt32();
                parts[i].Index = reader.ReadInt32();
                // Skip NAME magic
                reader.Stream.Seek(0x5, SeekMode.Current);
                parts[i].Name = reader.ReadString(0x23);
                
                // Skip AREA magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var areaversion = reader.ReadInt32();
                var areadata = reader.ReadBytes(0x10);

                // Skip ORGX magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var unk = reader.ReadInt32();
                parts[i].Sprite_positionX = reader.ReadInt32();

                // Skip ORGY magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                unk = reader.ReadInt32();
                parts[i].Sprite_positionY = reader.ReadInt32();

                // Skip TDBT magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                unk = reader.ReadInt32();
            }
            return parts;
        }
    }
}
