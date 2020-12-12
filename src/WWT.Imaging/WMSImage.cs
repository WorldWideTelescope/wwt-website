#nullable disable

using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace WWT.Imaging
{
    public enum ImageSource
    {
        OnMoon,
        WmsJpl,
        MarsAsu
    }

    public class WMSImage : IDisposable
    {
        private readonly double raMin;
        private readonly double decMax;
        private readonly double raMax;
        private readonly double decMin;
        private readonly double scaleX;
        private readonly double scaleY;

        private FastBitmap fastImage;

        public Bitmap Image { get; set; }

        public WMSImage(double raMin, double decMax, double raMax, double decMin)
        {
            this.raMin = raMin;
            this.decMin = decMin;
            this.raMax = raMax;
            this.decMax = decMax;
            scaleX = (this.raMax - this.raMin) / 512;
            scaleY = (this.decMax - this.decMin) / 512;
        }

        public void Dispose()
        {
            fastImage?.Dispose();
            Image?.Dispose();
        }

        public string GetImageUrl(string url, ImageSource source)
        {
            object[] args = new object[] { (raMin - 180), decMin, (raMax - 180), decMax, 512.0, 512.0, url };

            var formatString = source switch
            {
                ImageSource.OnMoon => "http://onmoon.jpl.nasa.gov/browse.cgi?WIDTH={4}&HEIGHT={5}&layers=Clementine&styles=&srs=IAU2000:30100&format=image/jpeg&bbox={0},{1},{2},{3}",
                ImageSource.WmsJpl => "http://wms.jpl.nasa.gov/wms.cgi?request=GetMap&layers=daily_planet&srs=EPSG:4326&format=image/jpeg&styles=&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={5}",
                ImageSource.MarsAsu => "http://ms.mars.asu.edu/?REQUEST=GetMap&SERVICE=WMS&VERSION=1.1.1&LAYERS={6}&STYLES=&FORMAT=image/png&BGCOLOR=0x000000&TRANSPARENT=FALSE&SRS=JMARS:1&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={5}&reaspect=false",
                _ => throw new NotImplementedException(),
            };

            return string.Format(formatString, args);
        }

        public void Lock()
        {
            this.fastImage = new FastBitmap(this.Image);
            this.fastImage.LockBitmap();
        }

        public void Unlock()
        {
            if (this.fastImage != null)
            {
                this.fastImage.UnlockBitmap();
                this.fastImage.Dispose();
                this.fastImage = null;
            }
        }

        public PixelData GetPixelDataAtRaDec(Vector2d raDec)
        {
            double x = Math.Max(0, Math.Min((raDec.X - raMin) / this.scaleX, 511));
            double y = Math.Max(0, Math.Min(511 - (raDec.Y - decMin) / this.scaleY, 511));

            return this.fastImage.GetFilteredPixel(x, y);
        }

        public string GetPixelDataAtRaDecString(Vector2d raDec)
        {
            double x = (raDec.X - raMin) / this.scaleX;
            double y = (raDec.Y - decMin) / this.scaleY;

            return string.Format("x={0},y={1}, scaleX={2}, scaleY={3}", x, y, scaleX, scaleY);
        }
    }
}
