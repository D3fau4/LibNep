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
            for (int i = 1; i < numberofentrys; i++)
            {
                Console.WriteLine(reader.Stream.Position.ToString("x"));
                parts[i] = new PART();
                // First PART
                var _magic = reader.ReadBytes(4);
                if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2] || magic[3] != _magic[3])
                    throw new Exception("Incorrect file format");

                parts[i].Version = reader.ReadInt32();
                parts[i].Index = reader.ReadInt32();

                // Skip NAME magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var Namelenght = reader.ReadInt32();
                parts[i].Name = reader.ReadString(Namelenght);
                
                // Skip AREA magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var arealenght = reader.ReadInt32();
                var areadata = reader.ReadBytes(arealenght);

                // Skip ORGX magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var ORGXLenght = reader.ReadInt32();
                parts[i].Sprite_positionX = reader.ReadInt32();

                // Skip ORGY magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var ORGYLenght = reader.ReadInt32();
                parts[i].Sprite_positionY = reader.ReadInt32();

                // Skip TDBT magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var TDBTLenght = reader.ReadInt32();
                int[] TDBTData = new int[TDBTLenght / 4];
                for (int x = 0; x < TDBTData.Length; x++)
                    TDBTData[x] = reader.ReadInt32();

                // Skip MYID magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var MYIDLenght = reader.ReadInt32();
                int[] MYIDData = new int[MYIDLenght / 4];
                for (int x = 0; x < MYIDData.Length; x++)
                    MYIDData[x] = reader.ReadInt32();

                // Skip PAID magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var PAIDLenght = reader.ReadInt32();
                int[] PAIDData = new int[PAIDLenght / 4];
                for (int x = 0; x < PAIDData.Length; x++)
                    PAIDData[x] = reader.ReadInt32();

                // Skip CHIP magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var CHIPLenght = reader.ReadInt32();
                int[] CHIPData = new int[CHIPLenght / 4];
                for (int x = 0; x < CHIPData.Length; x++)
                    CHIPData[x] = reader.ReadInt32();

                // Skip PCID magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var PCIDLenght = reader.ReadInt32();
                int[] PCIDData = new int[PCIDLenght / 4];
                for (int x = 0; x < PCIDData.Length; x++)
                    PCIDData[x] = reader.ReadInt32();

                // Skip SUCD magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var SUCDLenght = reader.ReadInt32();
                int[] SUCDData = new int[SUCDLenght / 4];
                for (int x = 0; x < SUCDData.Length; x++)
                    SUCDData[x] = reader.ReadInt32();

                // Skip POSX magic
                Console.WriteLine(reader.Stream.Position.ToString("x"));
                reader.Stream.Seek(0x4, SeekMode.Current);
                var POSXLenght = reader.ReadInt32();
                int[] POSXData = new int[POSXLenght / 4];
                for (int x = 0; x < POSXData.Length; x++)
                    POSXData[x] = reader.ReadInt32();

                reader.Stream.Seek(0x8, SeekMode.Current);

                // Skip POSY magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var POSYLenght = reader.ReadInt32();
                int[] POSYData = new int[POSYLenght / 4];
                for (int x = 0; x < POSYData.Length; x++)
                    POSYData[x] = reader.ReadInt32();

                // Padding (?)
                reader.Stream.Seek(0x8, SeekMode.Current);

                // Skip TRAN magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var TRANLenght = reader.ReadInt32();
                int[] TRANData = new int[TRANLenght / 4];
                for (int x = 0; x < TRANData.Length; x++)
                    TRANData[x] = reader.ReadInt32();

                // Padding (?)
                reader.Stream.Seek(0x8, SeekMode.Current);

                //Skip HIDE magic
                reader.Stream.Seek(0x4, SeekMode.Current);
                var HIDELenght = reader.ReadInt32();
                byte[] HIDEData = reader.ReadBytes(HIDELenght);
                
                reader.Stream.Seek(0x8, SeekMode.Current);
                var key = reader.ReadString(4);
                if (key != "UDAT" && key != "PART")
                    throw new Exception("Key desconocida");
            }
            return parts;
        }
    }
}
