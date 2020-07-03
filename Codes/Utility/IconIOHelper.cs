using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Bamboo
{
    static class IconIOHelper
    {
        public static BitmapData LoadPngIconFrame(Stream stream, int offset, int length)
        {
            using (var ms = new MemoryStream())
            {
                stream.Position = offset;
                stream.CopySegmentTo(ms, length);
                ms.Position = 0;

                return BitmapData.LoadStream(ms);
            }
        }

        public static BitmapData LoadBmpIconFrame(Stream stream, int offset, int length)
        {
            stream.Position = offset;
            using (var reader = new BinaryReader(stream, Encoding.Default, true))
            {
                var info = new BITMAPINFOHEADER
                {
                    biSize = reader.ReadInt32(),
                    biWidth = reader.ReadInt32(),
                    biHeight = reader.ReadInt32(),
                    biPlanes = reader.ReadInt16(),
                    biBitCount = reader.ReadInt16(),
                    biCompression = (BitmapCompression)reader.ReadInt32(),
                    biSizeImage = reader.ReadInt32(),
                    biXPelsPerMeter = reader.ReadInt32(),
                    biYPelsPerMeter = reader.ReadInt32(),
                    biClrUsed = reader.ReadInt32(),
                    biClrImportant = reader.ReadInt32(),
                };

                int height = Math.Abs(info.biHeight) / 2;

                BitmapData bitmapData = null;
                if (info.biCompression == BitmapCompression.BI_BITFIELDS)
                {
                    bitmapData = LoadBmpData_BITFIELDS(reader, info, height);
                }
                else
                {
                    int colorTableSize = (info.biBitCount <= 8 && info.biClrUsed == 0) ? (int)Math.Pow(2, info.biBitCount) : info.biClrUsed;
                    Color[] colorTable = null;
                    if (colorTableSize > 0)
                    {
                        colorTable = new Color[colorTableSize];
                        for (int i = 0; i < colorTableSize; i++)
                        {
                            var rgb = new RGBQUAD
                            {
                                rgbBlue = reader.ReadByte(),
                                rgbGreen = reader.ReadByte(),
                                rgbRed = reader.ReadByte(),
                                rgbReserved = reader.ReadByte()
                            };
                            colorTable[i] = Color.FromArgb(rgb.rgbRed, rgb.rgbGreen, rgb.rgbBlue);
                        }
                    }

                    if (info.biCompression == BitmapCompression.BI_RLE4)
                    {
                        bitmapData = LoadBmpData_RLE4(reader, info, height, colorTable);
                    }
                    else if (info.biCompression == BitmapCompression.BI_RLE8)
                    {
                        bitmapData = LoadBmpData_RLE8(reader, info, height, colorTable);
                    }
                    else if (colorTable != null)
                    {
                        bitmapData = LoadBitmapData(stream, info, height, colorTable);
                    }
                    else
                    {
                        bitmapData = LoadBitmapData(stream, info, height);
                    }
                }

                // load AND mask
                var andMask = new List<bool[]>();
                int stride = (info.biWidth + 31) / 32 * 4;
                for (int y = 0; y < height; y++)
                {
                    byte[] line = new byte[stride];
                    stream.Read(line, 0, line.Length);
                    var lineMask = new List<bool>();
                    foreach(var lb in line)
                    {
                        lineMask.Add((lb & 0b_1000_0000) == 0);
                        lineMask.Add((lb & 0b_0100_0000) == 0);
                        lineMask.Add((lb & 0b_0010_0000) == 0);
                        lineMask.Add((lb & 0b_0001_0000) == 0);
                        lineMask.Add((lb & 0b_0000_1000) == 0);
                        lineMask.Add((lb & 0b_0000_0100) == 0);
                        lineMask.Add((lb & 0b_0000_0010) == 0);
                        lineMask.Add((lb & 0b_0000_0001) == 0);
                    }
                    andMask.Add(lineMask.ToArray());
                }
                andMask.Reverse();
                if (info.biBitCount < 32)
                {
                    bitmapData.AndMaskToAlpha(andMask);
                }

                return bitmapData;
            }
        }

        private static BitmapData LoadBmpData_RLE4(BinaryReader reader, BITMAPINFOHEADER info, int height, Color[] colorTable)
        {
            var lines = new List<List<Color>>();
            for (int y = 0; y < height; y++)
            {
                var line = new List<Color>();
                int width = info.biWidth;
                while (width > 0)
                {
                    var type = reader.ReadByte();
                    if (type == 0x00)
                    {
                        var pixels = reader.ReadByte();
                        if (pixels > 0)
                        {
                            for (int i = 0; i < pixels; i += 2)
                            {
                                var colorByte = reader.ReadByte();
                                var color0 = colorByte >> 4;
                                var color1 = colorByte & 0x0F;

                                line.Add(colorTable[color0]);
                                width--;

                                if (i + 1 < pixels)
                                {
                                    line.Add(colorTable[color1]);
                                    width--;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        var colorByte = reader.ReadByte();
                        var color0 = colorByte >> 4;
                        var color1 = colorByte & 0x0F;

                        // in this case, the 'type' is repeat times now
                        for (int i = 0; i < type; i++)
                        {
                            Color color = (i % 2 == 0) ? colorTable[color0] : colorTable[color1];
                            line.Add(color);
                        }

                        width -= type;
                    }
                }

                lines.Add(line);
            }

            lines.Reverse();
            return new BitmapData(info.biWidth, lines);
        }

        private static BitmapData LoadBmpData_RLE8(BinaryReader reader, BITMAPINFOHEADER info, int height, Color[] colorTable)
        {
            var lines = new List<List<Color>>();
            for (int y = 0; y < height; y++)
            {
                var line = new List<Color>();
                int width = info.biWidth;
                while (width > 0)
                {
                    var type = reader.ReadByte();
                    if (type == 0x00)
                    {
                        var pixels = reader.ReadByte();
                        if (pixels > 0)
                        {
                            for (int i = 0; i < pixels; i += 2)
                            {
                                var colorByte = reader.ReadByte();
                                line.Add(colorTable[colorByte]);
                            }
                            width -= pixels;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        var colorByte = reader.ReadByte();
                        // in this case, the 'type' is repeat times now
                        for (int i = 0; i < type; i++)
                        {
                            line.Add(colorTable[colorByte]);
                        }

                        width -= type;
                    }
                }
                lines.Add(line);
            }

            lines.Reverse();
            return new BitmapData(info.biWidth, lines);
        }

        private static BitmapData LoadBmpData_BITFIELDS(BinaryReader reader, BITMAPINFOHEADER info, int height)
        {
            uint rMask = reader.ReadUInt32();
            uint gMask = reader.ReadUInt32();
            uint bMask = reader.ReadUInt32();

            int rMove = GetRightBorder(rMask);
            int gMove = GetRightBorder(gMask);
            int bMove = GetRightBorder(bMask);

            var lines = new List<List<Color>>();
            int stride = ((info.biWidth * info.biBitCount) + 31) / 32 * 4;
            var pixel = new byte[info.biBitCount / 8];
            var row = new byte[stride];
            for (int y = 0; y < height; y++)
            {
                var line = new List<Color>();
                if (reader.BaseStream.Read(row, 0, row.Length) > 0)
                {
                    for (int x = 0; x < row.Length; x += pixel.Length)
                    {
                        Buffer.BlockCopy(row, x, pixel, 0, pixel.Length);
                        var pixelData = BitConverter.ToUInt32(pixel, 0);
                        line.Add(Color.FromArgb(
                            (byte)(pixelData & rMask >> rMove),
                            (byte)(pixelData & gMask >> gMove),
                            (byte)(pixelData & bMask >> bMove)));
                    }
                }
                lines.Add(line);
            }

            lines.Reverse();
            return new BitmapData(info.biWidth, lines);
        }

        private static int GetRightBorder(uint mask)
        {
            int index = 0;
            uint f = 1;
            while ((mask & f) == 0)
            {
                f <<= 1;
                index++;
            }

            return index;
        }

        private static BitmapData LoadBitmapData(Stream stream, BITMAPINFOHEADER info, int height)
        {
            var lines = new List<List<Color>>();
            int stride = ((info.biWidth * info.biBitCount) + 31) / 32 * 4;
            var row = new byte[stride];
            for (int y = 0; y < height; y++)
            {
                if (stream.Read(row, 0, row.Length) == 0)
                {
                    break;
                }

                int i = 0;
                var line = new List<Color>();
                switch (info.biBitCount)
                {
                    case 16:
                        for (int x = 0; x < info.biWidth; x++)
                        {
                            ushort pixel = BitConverter.ToUInt16(row, i);
                            i += 2;

                            var b = (byte)((pixel & 0b_1111_1000_0000_0000) >> 11);
                            var g = (byte)((pixel & 0b_0000_0111_1100_0000) >> 11);
                            var r = (byte)((pixel & 0b_0000_0000_0011_1110) >> 11);
                            line.Add(Color.FromArgb(r, g, b));
                        }
                        break;
                    case 24:
                        for (int x = 0; x < info.biWidth; x++)
                        {
                            line.Add(Color.FromArgb(row[i + 2], row[i + 1], row[i]));
                            i += 3;
                        }
                        break;
                    case 32:
                        for (int x = 0; x < info.biWidth; x++)
                        {
                            line.Add(Color.FromArgb(row[i + 3], row[i + 2], row[i + 1], row[i]));
                            i += 4;
                        }
                        break;
                }

                lines.Add(line);
            }

            lines.Reverse();
            return new BitmapData(info.biWidth, lines);
        }

        private static BitmapData LoadBitmapData(Stream stream, BITMAPINFOHEADER info, int height, Color[] colorTable)
        {
            byte[] bit1_masks = new byte[] { 0b_1000_0000, 0b_0100_0000, 0b_0010_0000, 0b_0001_0000, 0b_0000_1000, 0b_0000_0100, 0b_0000_0010, 0b_0000_0001 };

            var lines = new List<List<Color>>();
            int stride = ((info.biWidth * info.biBitCount) + 31) / 32 * 4;
            var row = new byte[stride];
            for (int y = 0; y < height; y++)
            {
                if (stream.Read(row, 0, row.Length) > 0)
                {
                    var line = new List<Color>();
                    var width = info.biWidth;
                    switch (info.biBitCount)
                    {
                        case 1:
                            foreach (var bt in row)
                            {
                                for (int mk = 0; mk < bit1_masks.Length && width > 0; mk++)
                                {
                                    line.Add(colorTable[(bt & bit1_masks[mk]) > 0 ? 1 : 0]);
                                    width--;
                                }
                            }
                            break;
                        case 4:
                            foreach (var bt in row)
                            {
                                line.Add(colorTable[(bt & 0xF0) >> 4]);
                                width--;

                                if (width > 0)
                                {
                                    line.Add(colorTable[bt & 0x0F]);
                                    width--;
                                }
                            }
                            break;
                        case 8:
                            for (int x = 0; x < info.biWidth; x++)
                            {
                                line.Add(colorTable[row[x]]);
                            }
                            break;
                    }

                    lines.Add(line);
                }
            }

            lines.Reverse();
            return new BitmapData(info.biWidth, lines);
        }

#pragma warning disable IDE1006 // Naming Styles
        enum BitmapCompression
        {
            /// <summary>
            /// Normal
            /// </summary>
            BI_RGB = 0,
            /// <summary>
            /// Run-Length-Encoding 8
            /// </summary>
            BI_RLE8 = 1,
            /// <summary>
            /// Run-Length-Encoding 4
            /// </summary>
            BI_RLE4 = 2,
            /// <summary>
            /// MASK
            /// </summary>
            BI_BITFIELDS = 3
        }

        struct BITMAPINFOHEADER
        {
            public int biSize { get; set; }
            public int biWidth { get; set; }
            public int biHeight { get; set; }
            public short biPlanes { get; set; }
            public short biBitCount { get; set; }
            public BitmapCompression biCompression { get; set; }
            public int biSizeImage { get; set; }
            public int biXPelsPerMeter { get; set; }
            public int biYPelsPerMeter { get; set; }
            public int biClrUsed { get; set; }
            public int biClrImportant { get; set; }
        }

        struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}
