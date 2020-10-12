using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WWT.Providers
{
    public class MandelProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public MandelProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string filename = $@"{_options.DSSTileCache}\wwtcache\mandel\level{level}\{tileY}\{tileX}_{tileY}.png";
            string path = Path.GetDirectoryName(filename);

            if ((level < 32) && File.Exists(filename))
            {
                try
                {
                    context.Response.WriteFile(filename);
                }
                catch
                {
                }
            }
            else
            {
                double tileWidth = (4 / (Math.Pow(2, level)));
                double Sy = ((double)tileY * tileWidth) - 2;
                double Fy = Sy + tileWidth;
                double Sx = ((double)tileX * tileWidth) - 4;
                double Fx = Sx + tileWidth;

                context.Response.Clear();

                Color[] cs = new Color[256];
                {
                    try
                    {
                        Color[] c = new Color[256];
                        StreamReader sr = new StreamReader(Path.Combine(_options.WwtWebDir, "wwtweb", "colors.map"));
                        ArrayList lines = new ArrayList();
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            lines.Add(line);
                            line = sr.ReadLine();
                        }
                        int i = 0;
                        for (i = 0; i < Math.Min(256, lines.Count); i++)
                        {
                            string curC = (string)lines[i];
                            Color temp = Color.FromArgb(int.Parse(curC.Split(' ')[0]), int.Parse(curC.Split(' ')[1]), int.Parse(curC.Split(' ')[2]));
                            c[i] = temp;
                        }
                        for (int j = i; j < 256; j++)
                        {
                            c[j] = Color.White;
                        }
                        cs = c;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Invalid ColorMap file.", ex);
                    }
                }

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
                        b.SetPixel(s, z, looper == MAXITER ? Color.Black : cs[val]);
                        y += intigralY;
                    }
                    x += intigralX;
                }

                if (level < 32)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    b.Save(filename);
                }
                b.Save(context.Response.OutputStream, ImageFormat.Png);
                b.Dispose();
            }

            context.Response.End();
        }
    }
}
