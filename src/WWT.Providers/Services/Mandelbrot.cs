#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WWT.Providers.Services
{
    public class Mandelbrot : IMandelbrot
    {
        private readonly Color[] _colorMap;

        public Mandelbrot()
        {
            _colorMap = CreateColorMap();
        }

        public Stream CreateMandelbrot(int level, int tileX, int tileY)
        {
            using (var image = CreateMandelbrotBitmap(level, tileX, tileY))
            {
                return image.SaveToStream(ImageFormat.Png);
            }
        }

        private Bitmap CreateMandelbrotBitmap(int level, int tileX, int tileY)
        {
            double tileWidth = (4 / (Math.Pow(2, level)));
            double Sy = ((double)tileY * tileWidth) - 2;
            double Fy = Sy + tileWidth;
            double Sx = ((double)tileX * tileWidth) - 4;
            double Fx = Sx + tileWidth;

            int MAXITER = 100 + level * 100;

            var b = new Bitmap(256, 256);
            double x, y, x1, y1, xx, xmin, xmax, ymin, ymax = 0.0;
            int looper, s, z = 0;
            double intigralX, intigralY = 0.0;
            xmin = Sx;
            ymin = Sy;
            xmax = Fx;
            ymax = Fy;
            intigralX = (xmax - xmin) / 256;
            intigralY = (ymax - ymin) / 256;
            x = xmin;
            for (s = 0; s < 256; s++)
            {
                y = ymin;
                for (z = 0; z < 256; z++)
                {
                    x1 = 0;
                    y1 = 0;
                    looper = 0;
                    while (looper < MAXITER && ((x1 * x1) + (y1 * y1)) < 4)
                    {
                        looper++;
                        xx = (x1 * x1) - (y1 * y1) + x;
                        y1 = 2 * x1 * y1 + y;
                        x1 = xx;
                    }
                    double perc = looper / (256.0);
                    int val = looper % 254;
                    b.SetPixel(s, z, looper == MAXITER ? Color.Black : _colorMap[val]);
                    y += intigralY;
                }
                x += intigralX;
            }

            return b;
        }

        private static Color[] CreateColorMap()
        {
            var c = new Color[256];

            using (var stream = typeof(MandelProvider).Assembly.GetManifestResourceStream(typeof(Mandelbrot), "colors.map"))
            using (var sr = new StreamReader(stream))
            {
                var lines = new List<string>();
                var line = sr.ReadLine();

                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }

                int i = 0;
                for (i = 0; i < Math.Min(256, lines.Count); i++)
                {
                    var curC = lines[i];
                    var temp = Color.FromArgb(int.Parse(curC.Split(' ')[0]), int.Parse(curC.Split(' ')[1]), int.Parse(curC.Split(' ')[2]));
                    c[i] = temp;
                }

                for (int j = i; j < 256; j++)
                {
                    c[j] = Color.White;
                }
            }

            return c;
        }
    }
}
