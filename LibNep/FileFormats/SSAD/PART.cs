using System;
using System.Collections.Generic;
using System.IO;
using Yarhl.IO;

namespace LibNep.FileFormats.SSAD
{
    public class PART
    {
        public struct AREA
        {
            public int Size;
            public int UNK;
            public int X;
            public int Y;
        }

        public struct ORG
        {
            public int _X;
            public int[] X;
            public int _Y;
            public int[] Y;
        }

        public struct POS
        {
            public int _X;
            public int[] X;
            public int _Y;
            public int[] Y;
        }

        public struct SCA
        {
            public int _X;
            public int[] X;
            public int _Y;
            public int[] Y;
        }

        public readonly byte[] PART_magic = { 0x50, 0x41, 0x52, 0x54 };
        public readonly byte[] POSX_magic = { 0x50, 0x4F, 0x53, 0x58 };
        public readonly byte[] POSY_magic = { 0x50, 0x4F, 0x53, 0x59 };
        public readonly byte[] SCAX_magic = { 0x53, 0x43, 0x41, 0x58 };
        public readonly byte[] SCAY_magic = { 0x53, 0x43, 0x41, 0x59 };
        public int Version = 0;
        public int Index = 0;
        public string Name = "";
        public AREA _area;
        public ORG _org;
        public POS _pos;
        public SCA _sca;

        public List<PART> GetPARTs(DataReader reader, int numberofentrys)
        {
            List<PART> parts = new List<PART>();
            //reader.SkipPadding(0x20);
            var origin = reader.Stream.Position;
            for(int p = 0; p < numberofentrys; p++)
            {
                PART part = new PART();
                while (true)
                {
                    var _magic = reader.ReadBytes(4);
                    if (PART_magic[0] != _magic[0] || PART_magic[1] != _magic[1] || PART_magic[2] != _magic[2] || PART_magic[3] != _magic[3])
                    {
                        // No hacer nada
                    } else
                    {
                        Console.WriteLine(reader.Stream.Position.ToString("x"));
                        break;
                    } 
                }

                part.Version = reader.ReadInt32();
                part.Index = reader.ReadInt32();

                // NAME magic
                reader.Stream.Seek(0x4, SeekOrigin.Current); // Skip HEADER
                var Namelenght = reader.ReadInt32();
                part.Name = reader.ReadString(Namelenght);

                // AREA magic
                reader.Stream.Seek(0x4, SeekOrigin.Current); // Skip HEADER
                part._area.Size = reader.ReadInt32();
                part._area.UNK = reader.ReadInt32();
                part._area.X = reader.ReadInt32();
                part._area.Y = reader.ReadInt32();

                // ORGX
                reader.Stream.Seek(0x8, SeekOrigin.Current); // Skip HEADER
                var ORGXsize = reader.ReadInt32();
                if (ORGXsize > 4)
                {
                    part._org.X = new int[ORGXsize / 4];
                    for (int i = 0; i < _org.X.Length; i++)
                    {
                        part._org.X[i] = reader.ReadInt32();
                    }
                } 
                else
                {
                    part._org._X = reader.ReadInt32();
                }

                // ORGY
                reader.Stream.Seek(0x4, SeekOrigin.Current); // Skip HEADER
                var ORGYsize = reader.ReadInt32();
                if (ORGYsize > 4)
                {
                    part._org.Y = new int[ORGYsize / 4];
                    for (int i = 0; i < part._org.X.Length; i++)
                    {
                        part._org.Y[i] = reader.ReadInt32();
                    }
                }
                else
                {
                    part._org._Y = reader.ReadInt32();
                }

                // Search POSX
                while (true)
                {
                    var bytes = reader.ReadBytes(4);
                    if (bytes[0] != POSX_magic[0] || bytes[1] != POSX_magic[1] || bytes[2] != POSX_magic[2] || bytes[3] != POSX_magic[3]) 
                    {
                        // No hacer nada
                    } 
                    else
                    {
                        var POSXsize = reader.ReadInt32();
                        if (POSXsize > 4)
                        {
                            part._pos.X = new int[POSXsize / 4];
                            for (int i = 0; i < part._pos.X.Length; i++)
                            {
                                part._pos.X[i] = reader.ReadInt32();
                            }
                            break;
                        }
                        else
                        {
                            part._pos._X = reader.ReadInt32();
                            break;
                        }
                    }
                }

                // Search POSY
                while (true)
                {
                    var bytes = reader.ReadBytes(4);
                    if (bytes[0] != POSY_magic[0] || bytes[1] != POSY_magic[1] || bytes[2] != POSY_magic[2] || bytes[3] != POSY_magic[3])
                    {
                        // No hacer nada
                    }
                    else
                    {
                        var POSYsize = reader.ReadInt32();
                        if (POSYsize > 4)
                        {
                            part._pos.Y = new int[POSYsize / 4];
                            for (int i = 0; i < part._pos.Y.Length; i++)
                            {
                                part._pos.Y[i] = reader.ReadInt32();
                            }
                            break;
                        }
                        else
                        {
                            part._pos._Y = reader.ReadInt32();
                            break;
                        }
                    }
                }

                // Search SCAX
                while (true)
                {
                    var bytes = reader.ReadBytes(4);
                    if (bytes[0] != SCAX_magic[0] || bytes[1] != SCAX_magic[1] || bytes[2] != SCAX_magic[2] || bytes[3] != SCAX_magic[3])
                    {
                        // No hacer nada
                    }
                    else
                    {
                        var SCAXsize = reader.ReadInt32();
                        if (SCAXsize > 4)
                        {
                            part._sca.X = new int[SCAXsize / 4];
                            for (int i = 0; i < part._sca.X.Length; i++)
                            {
                                part._sca.X[i] = reader.ReadInt32();
                            }
                            break;
                        }
                        else
                        {
                            part._sca._X = reader.ReadInt32();
                            break;
                        }
                    }
                }

                // Search SCAY
                while (true)
                {
                    var bytes = reader.ReadBytes(4);
                    if (bytes[0] != SCAY_magic[0] || bytes[1] != SCAY_magic[1] || bytes[2] != SCAY_magic[2] || bytes[3] != SCAY_magic[3])
                    {
                        // No hacer nada
                    }
                    else
                    {
                        var SCAYsize = reader.ReadInt32();
                        if (SCAYsize > 4)
                        {
                            part._sca.Y = new int[SCAYsize / 4];
                            for (int i = 0; i < part._sca.Y.Length; i++)
                            {
                                part._sca.Y[i] = reader.ReadInt32();
                            }
                            break;
                        }
                        else
                        {
                            part._sca._Y = reader.ReadInt32();
                            break;
                        }
                    }
                }

                parts.Add(part);
            }
            return parts;
        }
    }
}
