using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.Images.DXT
{
    internal abstract class DxtTexel
    {
        public Color[] Pixels { get; private set; }

        protected DxtTexel(ushort packedC0, ushort packedC1, uint colorIndices)
        {
            var colors = InterpolateColors(packedC0, packedC1);
            SetPixels(colors, colorIndices);
        }

        public abstract Color[] InterpolateColors(ushort packedC0, ushort packedC1);

        public void SetPixels(Color[] colors, uint colorIndices)
        {
            Pixels = new Color[16];
            for (int i = 0; i < 16; i++, colorIndices >>= 2)
            {
                Pixels[i] = colors[colorIndices & 0b11];
            }
        }
    }
}
