using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/StarChart.aspx")]
    public class StarChartProvider : StarChart
    {
        public override string ContentType => ContentTypes.Png;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            double lat = double.Parse(context.Request.Params["lat"]);
            double lng = double.Parse(context.Request.Params["lng"]);
            double ra = double.Parse(context.Request.Params["ra"]);
            double dec = double.Parse(context.Request.Params["dec"]);
            double time = 0;
            int width = int.Parse(context.Request.Params["width"]);
            int height = int.Parse(context.Request.Params["height"]);

            if (context.Request.Params["jtime"] != null)
            {
                time = double.Parse(context.Request.Params["jtime"]);
            }
            else
            {
                if (context.Request.Params["time"] != null)
                {
                    time = Calc.ToJulian(DateTime.Parse(context.Request.Params["time"]));
                }
                else
                {
                    time = Calc.ToJulian(DateTime.Now.ToUniversalTime());
                }
            }


            Bitmap chart = GetChart(lat, lng, time, ra, dec, width, height);
            chart.Save(context.Response.OutputStream, ImageFormat.Png);
            chart.Dispose();

            return Task.CompletedTask;
        }
    }
}
