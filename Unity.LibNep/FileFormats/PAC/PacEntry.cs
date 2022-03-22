using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LibNep.Utils;

namespace LibNep.FileFormats.PAC
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x14)]
    public unsafe struct DwPackHeader
    {
        public const int SIZE = 0x14;
        public const ulong SIGNATURE = 0x4B4341505F5744;

        public ulong Signature; // DW_PACK\0
        public int Field08;
        public int FileCount;
        public int Index;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x120)]
    public unsafe struct PacEntry
    {
        public const int PATH_LENGTH = 260;
        public const int SIZE = 0x120;

        public int Field00;
        public short Index;
        public short PackIndex;
        public fixed byte PathBytes[PATH_LENGTH];
        public int Field104;
        public int CompressedSize;
        public int UncompressedSize;
        public int Flags;
        public int DataOffset;

        public string Path
        {
            get
            {
                fixed (byte* pathBytes = PathBytes)
                    return EncodingCache.ShiftJIS.GetString(NativeStringHelper.AsSpan(pathBytes));
            }
            set
            {
                fixed (byte* pathBytes = PathBytes)
                {
                    Unsafe.InitBlock(pathBytes, 0, PATH_LENGTH);
                    EncodingCache.ShiftJIS.GetBytes(value.AsSpan(), new Span<byte>(pathBytes, PATH_LENGTH));
                }
            }
        }
    }
}
