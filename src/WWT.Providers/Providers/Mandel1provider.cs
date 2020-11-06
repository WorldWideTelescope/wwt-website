using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/mandel1.aspx")]
    public class Mandel1Provider : RequestProvider
    {
        public override string ContentType => ContentTypes.Jpeg;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            double tileWidth = (4 / (Math.Pow(2, level)));
            double Sy = ((double)tileY * tileWidth) - 2;
            double Fy = Sy + tileWidth;
            double Sx = ((double)tileX * tileWidth) - 4;
            double Fx = Sx + tileWidth;


            context.Response.Clear();



            Bitmap b = new Bitmap(256, 256);
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
                    while (looper < 2048 && ((x1 * x1) + (y1 * y1)) < 4)
                    {
                        looper++;
                        xx = (x1 * x1) - (y1 * y1) + x;
                        y1 = 2 * x1 * y1 + y;
                        x1 = xx;
                    }

                    b.SetPixel(s, z, ((looper % 2) == 1) ? Color.White : Color.Black);
                    y += intigralY;
                }
                x += intigralX;
            }

            b.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            b.Dispose();
            context.Response.End();

            return Task.CompletedTask;
        }
    }
}
