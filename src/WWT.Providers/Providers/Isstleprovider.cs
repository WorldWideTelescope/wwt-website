using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/isstle.aspx")]
    public class IsstleProvider : isstle
    {
        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string url = "";

            {
                url = "http://spaceflight.nasa.gov/realdata/sightings/SSapplications/Post/JavaSSOP/orbit/ISS/SVPOST.html";

                try
                {
                    string reply = (string)context.Cache["WWTISSTLE"];
                    DateTime date = DateTime.Now;
                    TimeSpan ts = new TimeSpan(100, 0, 0, 0);
                    if (context.Cache["WWTISSTLDATE"] != null)
                    {
                        date = (DateTime)context.Cache["WWTISSTLDATE"];
                        ts = DateTime.Now - date;
                    }

                    if (String.IsNullOrEmpty(reply) || ts.TotalDays > .5 || context.Request.Params["refresh"] != null)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            string data = wc.DownloadString(url);

                            string[] lines = data.Split(new char[] { '\n', '\r' });
                            string line1 = "";
                            string line2 = "";
                            for (int i = 0; i < lines.Length; i++)
                            {
                                lines[i] = lines[i].Trim();
                                if (lines[i].Length == 69 && IsTLECheckSumGood(lines[i]))
                                {
                                    if (line1.Length == 0 && lines[i].Substring(0, 1) == "1")
                                    {
                                        line1 = lines[i];
                                    }
                                    if (line2.Length == 0 && lines[i].Substring(0, 1) == "2")
                                    {
                                        line2 = lines[i];
                                    }
                                }
                            }
                            if (line1 == "" || line2 == "")
                            {
                                reply = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
                                reply += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
                                reply += "Cached during government shutdown";

                            }
                            else
                            {

                                reply = line1 + "\n" + line2 + "\nLaste Updated:" + DateTime.Now;
                            }

                            context.Cache["WWTISSTLE"] = reply;
                            context.Cache["WWTISSTLDATE"] = DateTime.Now;
                        }
                    }

                    await context.Response.WriteAsync(reply, token);
                }
                catch (Exception)
                {
                    string reply = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
                    reply += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
                    reply += "Cached during NASA Downtime";

                    await context.Response.WriteAsync(reply, token);
                }
            }
        }
    }
}
