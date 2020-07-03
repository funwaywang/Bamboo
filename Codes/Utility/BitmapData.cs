using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Bamboo
{
    public class BitmapData
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int Stride;
        public readonly byte[] BitData;
        public readonly int PixelBytes = 4;

        public BitmapData(Bitmap bitmap, Rectangle? rectangle = null)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException();
            }

            Rectangle rect;
            if (rectangle != null && rectangle.Value.Width > 0 && rectangle.Value.Height > 0)
            {
                rect = new Rectangle(Math.Max(0, rectangle.Value.X), Math.Max(0, rectangle.Value.Y), Math.Min(bitmap.Width, rectangle.Value.Width), Math.Min(bitmap.Height, rectangle.Value.Height));
            }
            else
            {
                rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            }
            Width = rect.Width;
            Height = rect.Height;

            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                PixelBytes = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                Stride = Math.Abs(data.Stride);
                BitData = new byte[Stride * data.Height];
                Marshal.Copy(data.Scan0, BitData, 0, BitData.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        public BitmapData(int width, int height, byte[] data)
        {
            Width = width;
            Height = height;
            Stride = width * PixelBytes;
            BitData = data;
        }

        public BitmapData(int width, int height, IEnumerable<Color> pixels)
        {
            Width = width;
            Height = height;
            Stride = width * 4;
            BitData = (from c in pixels
                       from b in new byte[] { c.B, c.G, c.R, c.A }
                       select b).ToArray();
        }

        public BitmapData(int width, IEnumerable<IEnumerable<Color>> lines)
        {
            Width = width;
            Stride = width * 4;
            Height = lines.Count();
            BitData = (from l in lines
                       from c in l
                       from b in new byte[] { c.B, c.G, c.R, c.A }
                       select b).ToArray();
        }

        public virtual IEnumerable<ColorPosition> GetColors()
        {
            var lineP = 0;
            var position = 0;
            for (int y = 0; y < Height; y++)
            {
                var p = lineP;
                for (int x = 0; x < Width; x++)
                {
                    yield return new ColorPosition(Color.FromArgb(BitData[p + 3], BitData[p + 2], BitData[p + 1], BitData[p]), position);
                    position++;
                    p += PixelBytes;
                }
                lineP += Stride;
            }
        }

        public virtual Color GetPixel(int x, int y)
        {
            x = Math.Max(0, Math.Min(Width - 1, x));
            y = Math.Max(0, Math.Min(Height - 1, y));

            var span = new byte[4];
            Buffer.BlockCopy(BitData, y * Stride + x * PixelBytes, span, 0, 4);
            return Color.FromArgb(span[3], span[2], span[1], span[0]);
        }

        public virtual bool TryGetPixel(int x, int y, out Color? color)
        {
            color = null;
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            var span = new byte[4];
            Buffer.BlockCopy(BitData, y * Stride + x * PixelBytes, span, 0, 4);
            color = Color.FromArgb(span[3], span[2], span[1], span[0]);
            return true;
        }

        public virtual bool TryGetPixel(int x, int y, out System.Windows.Media.Color? color)
        {
            color = null;
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            var span = new byte[4];
            Buffer.BlockCopy(BitData, y * Stride + x * PixelBytes, span, 0, 4);
            color = System.Windows.Media.Color.FromArgb(span[3], span[2], span[1], span[0]);
            return true;
        }

        public static Task<BitmapData> LoadFileAsync(string filename)
        {
            return Task.Factory.StartNew(() => LoadFile(filename));
        }

        public static BitmapData LoadFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                using (var image = Image.FromFile(filename))
                {
                    using (var bitmap = new Bitmap(image))
                    {
                        return new BitmapData(bitmap);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public static Task<BitmapData> LoadStreamAsync(Stream stream)
        {
            return Task.Factory.StartNew(() => LoadStream(stream));
        }

        public static BitmapData LoadStream(Stream stream)
        {
            using (var image = Image.FromStream(stream))
            {
                using (var bitmap = new Bitmap(image))
                {
                    return new BitmapData(bitmap);
                }
            }
        }

        public Point? GetPoint(int position)
        {
            if (position >= 0 && position < Width * Height)
            {
                return new Point(position % Width, position / Width);
            }
            else
            {
                return null;
            }
        }

        public virtual byte[] GetClip(int x, int y, int width, int height)
        {
            width = Math.Min(Width - x, width);
            height = Math.Min(Height - y, height);

            var line = width * PixelBytes;
            var buffer = new byte[line * height];
            var ps = y * Stride + x * PixelBytes;
            var pd = 0;
            for (var l = 0; l < height; l++)
            {
                Buffer.BlockCopy(BitData, ps, buffer, pd, line);
                ps += Stride;
                pd += line;
            }

            return buffer;
        }

        public byte[] GetClip(Rectangle rect)
        {
            return GetClip(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static int CalculateStride(int imageWidth, int pixelBytes)
        {
            return ((imageWidth * pixelBytes + 3) / 4) * 4;
        }

        public static byte[] SimpleZoomIn(byte[] data, int sourceWidth, int sourceHeight, int zoom, int pixelBytes = 4)
        {
            if (zoom <= 1)
            {
                return data;
            }
            var sourceStride = CalculateStride(sourceWidth, pixelBytes);

            var width = sourceWidth * zoom;
            var height = sourceHeight * zoom;
            var stride = CalculateStride(width, pixelBytes);
            var buffer = new byte[stride * height];
            var sourceIndex = 0;
            for (int y = 0; y < sourceHeight; y++)
            {
                var si = sourceIndex;
                for (int x = 0; x < sourceWidth; x++)
                {
                    // zoom out point [x,y]
                    int dStart = (y * zoom * stride) + (x * zoom * pixelBytes);
                    for (int zy = 0; zy < zoom; zy++)
                    {
                        int di = dStart;
                        for (int zx = 0; zx < zoom; zx++)
                        {
                            Buffer.BlockCopy(data, si, buffer, di, pixelBytes);
                            di += pixelBytes;
                        }
                        dStart += stride;
                    }

                    si += pixelBytes;
                }
                sourceIndex += sourceStride;
            }

            return buffer;
        }

        public void AndMaskToAlpha(List<bool[]> andMask)
        {
            var p = 0;
            for (int y = 0; y < Height; y++)
            {
                var pl = p;
                var lineMask = andMask[y];
                for (int x = 0; x < Width; x++)
                {
                    BitData[pl + 3] = lineMask[x] ? (byte)0xFF : (byte)0x00;
                    pl += PixelBytes;
                }

                p += Stride;
            }
        }
    }

    public class ClipBitmapData : BitmapData
    {
        private readonly BitmapData source;
        private readonly Rectangle clip;

        public ClipBitmapData(BitmapData source, Rectangle clip)
            : base(clip.Width, clip.Height, source.BitData)
        {
            this.source = source;
            this.clip = clip;
        }

        public override IEnumerable<ColorPosition> GetColors()
        {
            var position = 0;
            int lineP = clip.Y * source.Stride;
            for (int y = 0; y < clip.Height; y++)
            {
                var p = lineP + clip.X * PixelBytes;
                for (int x = 0; x < clip.Width; x++)
                {
                    yield return new ColorPosition(Color.FromArgb(BitData[p + 3], BitData[p + 2], BitData[p + 1], BitData[p]), position);
                    position++;
                    p += PixelBytes;
                }
                lineP += source.Stride;
            }
        }

        public override Color GetPixel(int x, int y)
        {
            return source.GetPixel(x + clip.X, y + clip.Y);
        }

        public override bool TryGetPixel(int x, int y, out Color? color)
        {
            return source.TryGetPixel(x + clip.X, y + clip.Y, out color);
        }

        public override bool TryGetPixel(int x, int y, out System.Windows.Media.Color? color)
        {
            return source.TryGetPixel(x + clip.X, y + clip.Y, out color);
        }

        public override byte[] GetClip(int x, int y, int width, int height)
        {
            return base.GetClip(x + clip.X, y + clip.Y, width, height);
        }
    }

    public struct ColorPosition
    {
        public Color Color;

        public int Position;

        public ColorPosition(Color color, int position)
        {
            Color = color;
            Position = position;
        }
    }
}
