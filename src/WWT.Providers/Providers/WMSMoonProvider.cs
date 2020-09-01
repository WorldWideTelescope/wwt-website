using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class WMSMoonProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            bool debug = context.Request.Params["debug"] != null;
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string wmsUrl = values[3];
            string dirPart = Math.Abs(wmsUrl.GetHashCode()).ToString();
            string filename;
            string path;
            filename = String.Format("\\\\wwt-sql01\\DSSTileCache\\WMS\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, dirPart);
            path = String.Format("\\\\wwt-sql01\\DSSTileCache\\WMS\\{3}\\{0}\\{2}", level, tileX, tileY, dirPart);

            if (level > 15)
            {
                context.Response.Write("No image");
                context.Response.End();
                return;
            }

            if (File.Exists(filename))
            {
                try
                {
                    context.Response.WriteFile(filename);
                    return;
                }
                catch
                {
                }
            }
            else
            {
                ToastTileMap map = new ToastTileMap(level, tileX, tileY);
                Int32 sqSide = 256;

                Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
                FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
                WMSImage sdim = new WMSImage(map.raMin, map.decMax, map.raMax, map.decMin);
                if (debug)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(sdim.LoadImage(wmsUrl, true));
                    context.Response.End();
                    return;
                }
                sdim.LoadImage(wmsUrl, false);
                sdim.Lock();

                bmpOutputFast.LockBitmap();
                // Fill up bmp from sdim
                // context.Response.Clear();
                //  context.Response.ContentType = "text/plain";         
                Vector2d vxy, vradec;
                unsafe
                {
                    PixelData* pPixel;
                    for (int y = 0; y < sqSide; y++)
                    {
                        pPixel = bmpOutputFast[0, y];
                        vxy.Y = (y / 255.0);
                        for (int x = 0; x < sqSide; x++)
                        {
                            vxy.X = (x / 255.0);
                            vradec = map.PointToRaDec(vxy.X, vxy.Y);
                            *pPixel = sdim.GetPixelDataAtRaDec(vradec);

                            //context.Response.Write(sdim.GetPixelDataAtRaDecString(vradec));
                            //context.Response.Write("\n");


                            pPixel++;
                        }
                    }
                }
                //   context.Response.End();

                //sdim.Unlock();
                bmpOutputFast.UnlockBitmap();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                bmpOutput.Save(filename, ImageFormat.Png);
                bmpOutput.Dispose();
                try
                {
                    context.Response.WriteFile(filename);
                }
                catch
                {
                }

            }

            context.Response.End();
        }
    }
}
