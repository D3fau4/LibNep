using System;
using System.IO;

namespace LibNep.Images
{
    public static class DxtDecompressor
    {
        public static byte[] DecompressDxt(byte[] compressed, int width, int height, CompressionType compression)
        {
            using (var memoryStream = new MemoryStream(compressed))
            {
                return DecompressDxt(memoryStream, width, height, compression);
            }
        }

        public static byte[] DecompressDxt(Stream compressed, int width, int height, CompressionType compression)
        {
            if (compressed == null) throw new ArgumentNullException(nameof(compressed));
            if (!compressed.CanRead) throw new ArgumentException(nameof(compressed));
            if ((compressed.Length - compressed.Position) * 8 < width * height * 4) throw new ArgumentException($"{nameof(compressed)} does not contain enough data for specified size.", nameof(compressed));
            if (!Enum.IsDefined(typeof(CompressionType), compression)) throw new ArgumentException("Invalid compression specified.", nameof(compression));

            byte[] image = new byte[height * width * 4];

            using (var reader = new BinaryReader(compressed))
            {
                for (int baseY = 0; baseY < height; baseY += 4)
                {
                    for (int baseX = 0; baseX < width; baseX += 4)
                    {
                        DXT.DxtTexel texel;

                        switch (compression)
                        {
                            case CompressionType.DXT1:
                                texel = new DXT.Dxt1Texel(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt32());
                                break;
                            case CompressionType.DXT5:
                                texel = new DXT.Dxt5Texel(reader.ReadByte(),
                                    reader.ReadByte(),
                                    (ulong)reader.ReadUInt16() << 0 | (ulong)reader.ReadUInt32() << 16,
                                    reader.ReadUInt16(),
                                    reader.ReadUInt16(),
                                    reader.ReadUInt32());
                                break;
                            default:
                                throw new InvalidOperationException($"Unreachable code reached. {nameof(compression)}'s value ({compression}) is incorrect.");
                        }

                        var currentPixelIndex = 0;
                        for (int y = baseY; y < baseY + 4 && y < height; y++)
                        {
                            for (int x = baseX; x < baseX + 4 && x < width; x++)
                            {
                                int currentLocation = y * width * 4 + x * 4;
                                image[currentLocation + 0] = texel.Pixels[currentPixelIndex].B;
                                image[currentLocation + 1] = texel.Pixels[currentPixelIndex].G;
                                image[currentLocation + 2] = texel.Pixels[currentPixelIndex].R;
                                image[currentLocation + 3] = texel.Pixels[currentPixelIndex].A;
                                currentPixelIndex++;
                            }
                        }
                    }
                }
            }

            return image;
        }
    }
}