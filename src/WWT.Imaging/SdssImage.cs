#nullable disable

// OctSetTest.SdssImage
using OctSetTest;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using WWTWebservices;

namespace OctSetTest
{
	public class SdssImage
	{
		private const double D2R = Math.PI / 180.0;

		private double decCenter;

		private FastBitmap fastImage;

		public Bitmap image;

		private const double ImageSizeX = 512.0;

		private const double ImageSizeY = 512.0;

		private double raCenter;

		private double scale;

		private double xoff;

		private double yoff;

		private bool dr12;

		public SdssImage(double raOfCenter, double decOfCenter, double decMaxMinusMin)
		{
			scale = decMaxMinusMin;
			xoff = 256.0;
			yoff = 256.0;
			raCenter = raOfCenter;
			decCenter = decOfCenter;
		}

		public SdssImage(double raLeft, double decTop, double raRight, double decBottom)
		{
			scale = Math.Abs(decBottom - decTop) / 500.0;
			xoff = 256.0;
			yoff = 256.0;
			raCenter = (raLeft + raRight) / 2.0;
			decCenter = (decBottom + decTop) / 2.0;
		}

		public SdssImage(double raLeft, double decTop, double raRight, double decBottom, bool dr12)
		{
			this.dr12 = true;
			scale = Math.Abs(decBottom - decTop) / 500.0;
			xoff = 256.0;
			yoff = 256.0;
			raCenter = (raLeft + raRight) / 2.0;
			decCenter = (decBottom + decTop) / 2.0;
		}

		public static string FormatDMS(double angle)
		{
			try
			{
				int num = (int)angle;
				double num2 = (angle - (double)(int)angle) * 60.0;
				double num3 = (num2 - (double)(int)num2) * 60.0;
				return $"{num:00}:{Math.Abs((int)num2):00}:{Math.Abs((int)num3):00}";
			}
			catch
			{
				return "";
			}
		}

		public Color GetPixelAtRaDec(Vector2d raDec)
		{
			Vector2d vectord = default(Vector2d);
			vectord.Y = yoff - (raDec.Y - decCenter) / scale;
			vectord.X = xoff - (raDec.X - raCenter) * Math.Cos(raDec.Y * (Math.PI / 180.0)) / scale;
			PixelData filteredPixel = fastImage.GetFilteredPixel(vectord.X, vectord.Y);
			return Color.FromArgb(filteredPixel.red, filteredPixel.green, filteredPixel.blue);
		}

		public PixelData GetPixelDataAtRaDec(Vector2d raDec)
		{
			return fastImage.GetFilteredPixel(xoff - (raDec.X - raCenter) * Math.Cos(raDec.Y * (Math.PI / 180.0)) / scale, yoff - (raDec.Y - decCenter) / scale);
		}

		public Point GetPointAtRaDec(Vector2d raDec)
		{
			double num = yoff - (raDec.Y - decCenter) / scale;
			return new Point((int)(xoff - (raDec.X - raCenter) * Math.Cos(raDec.Y * (Math.PI / 180.0)) / scale), (int)num);
		}

		public Vector2d GetRaDecForPoint(Vector2d point)
		{
			double x = 0.0;
			double y = 0.0;
			y = decCenter - (point.Y - yoff) * scale;
			x = raCenter - (point.X - xoff) * scale / Math.Cos(y * (Math.PI / 180.0));
			if (x > 360.0)
			{
				x -= 360.0;
			}
			if (x < 0.0)
			{
				x += 360.0;
			}
			return new Vector2d(x, y);
		}

		public Vector2d GetVector2dAtRaDec(Vector2d raDec)
		{
			return new Vector2d(xoff - (raDec.X - raCenter) * Math.Cos(raDec.Y * (Math.PI / 180.0)) / scale, yoff - (raDec.Y - decCenter) / scale);
		}

		public void LoadImage()
		{
			object[] args = new object[5]
			{
			raCenter,
			decCenter,
			(scale * 3600.0).ToString(),
			512.0,
			512.0
			};
			string address = string.Format("http://skyservice.pha.jhu.edu/dr6/imgcutout/getjpeg.aspx?ra={0}&dec={1}&scale={2}&width={3}&height={4}&opt=&query=", args);
			if (dr12)
			{
				address = string.Format("http://skyservice.pha.jhu.edu/DR12/ImgCutout/getjpeg.aspx?ra={0}&dec={1}&scale={2}&width={3}&height={4}&opt=&query=", args);
			}
			byte[] data = new WebClient().DownloadData(address);
			if (data.Length > 8000)
			{
				using (MemoryStream stream = new MemoryStream(data))
				{
					image = new Bitmap(stream);
				}
			}
			else
			{
				image = null;
			}
		}

		public void Lock()
		{
			fastImage = new FastBitmap(image);
			fastImage.LockBitmap();
		}

		public void Unlock()
		{
			if (fastImage != null)
			{
				fastImage.UnlockBitmap();
				fastImage.Dispose();
				fastImage = null;
			}
		}
	}
}
