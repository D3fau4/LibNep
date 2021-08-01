using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.Images.DXT
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Color
    {
        [FieldOffset(0x00)] public uint Quad;
        [FieldOffset(0x00)] public byte B;
        [FieldOffset(0x01)] public byte G;
        [FieldOffset(0x02)] public byte R;
        [FieldOffset(0x03)] public byte A;

        internal static Color FromRgb565(ushort color)
        {
            var argb = new Color();

            var r = (color >> 11) & 0b11111;
            r = (r << 3) | (r >> 2);

            var g = (color >> 5) & 0b111111;
            g = (g << 2) | (g >> 4);

            var b = color & 0b11111;
            b = (b << 3) | (b >> 2);

            argb.Quad = unchecked((uint)(0xFF << 24 | r << 16 | g << 8 | b));

            return argb;
        }
    }
}
