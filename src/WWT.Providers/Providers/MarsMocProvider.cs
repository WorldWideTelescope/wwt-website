using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class MarsMocProvider : MarsMoc
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            //  string dataset = values[3];

            Bitmap output = new Bitmap(256, 256);
            Graphics g = Graphics.FromImage(output);

            Bitmap bmp1 = null;
            Bitmap bmp2 = null;

            // TODO: This level was set to 15 before. Should identify a better to know if a level is beyond the dataset without hardcoding.
            if (level > 14)
            {
                context.Response.StatusCode = 404;
                return;
            }

            int ll = level;
            int xx = tileX;
            int yy = tileY;
            if (ll > 8)
            {
                int levelDif = ll - 8;
                int scale = (int)Math.Pow(2, levelDif);
                int tx = xx / scale;
                int ty = yy / scale;

                int offsetX = (xx - (tx * scale)) * (256 / scale);
                int offsetY = (yy - (ty * scale)) * (256 / scale);
                float width = (256 / scale);
                float height = width;
                if ((width + offsetX) >= 255)
                {
                    width -= 1;
                }
                if ((height + offsetY) >= 255)
                {
                    height -= 1;
                }

                bmp1 = new Bitmap(PlateFile2.GetFileStream(Path.Combine(ConfigurationManager.AppSettings["WWTTilesDir"], "marsbasemap.plate"), -1, 8, tx, ty));

                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                g.DrawImage(bmp1, new RectangleF(0, 0, 256, 256), new RectangleF(offsetX, offsetY, width, height), GraphicsUnit.Pixel);

            }
            else
            {
                bmp1 = new Bitmap(PlateFile2.GetFileStream(Path.Combine(ConfigurationManager.AppSettings["WWTTilesDir"], "marsbasemap.plate"), -1, ll, xx, yy));
                g.DrawImageUnscaled(bmp1, new Point(0, 0));
            }


            // try
            {
                bmp2 = LoadMoc(ll, xx, yy);

                g.DrawImageUnscaled(bmp2, new Point(0, 0));
            }
            // catch
            {
            }

            g.Flush();
            g.Dispose();


            bmp1.Dispose();
            if (bmp2 != null)
            {
                bmp2.Dispose();
            }

            output.Save(context.Response.OutputStream, ImageFormat.Png);


            output.Dispose();
        }
    }
}
