using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/isstle.aspx")]
    public class IsstleProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        const string theUrl = "https://api.wheretheiss.at/v1/satellites/25544/tles?format=text";

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string reply = "";

            try
            {
                reply = (string) context.Cache["WWTISSTLE"];
                DateTime date = DateTime.Now;
                TimeSpan ts = new TimeSpan(100, 0, 0, 0);

                if (context.Cache["WWTISSTLEDATE"] != null)
                {
                    date = (DateTime) context.Cache["WWTISSTLEDATE"];
                    ts = DateTime.Now - date;
                }

                if (String.IsNullOrEmpty(reply) || ts.TotalDays > .5 || context.Request.Params["refresh"] != null)
                {
                    using (WebClient wc = new WebClient())
                    {
                        string data = wc.DownloadString(theUrl);
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
                            throw new SystemException("TLE webservice response did not include valid TLE data");
                        }

                        reply = line1 + "\n" + line2 + "\nLast Updated:" + DateTime.Now;
                        context.Cache["WWTISSTLE"] = reply;
                        context.Cache["WWTISSTLEDATE"] = DateTime.Now;
                    }
                }
            }
            catch (Exception)
            {
                reply = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
                reply += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
                reply += "WARNING: cached response due to backend failure";
            }

            await context.Response.WriteAsync(reply, token);
        }

        static bool IsTLECheckSumGood(string line)
        {
            if (line.Length != 69)
            {
                return false;
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++)
            {
                switch (line[i])
                {
                    case '1':
                        checksum += 1;
                        break;
                    case '2':
                        checksum += 2;
                        break;
                    case '3':
                        checksum += 3;
                        break;
                    case '4':
                        checksum += 4;
                        break;
                    case '5':
                        checksum += 5;
                        break;
                    case '6':
                        checksum += 6;
                        break;
                    case '7':
                        checksum += 7;
                        break;
                    case '8':
                        checksum += 8;
                        break;
                    case '9':
                        checksum += 9;
                        break;
                    case '-':
                        checksum += 1;

                        break;
                }
            }
            return ('0' + (char)(checksum % 10)) == line[68];
        }
    }
}
