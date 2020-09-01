using WWTWebservices;

namespace WWT.Providers
{
    public class testfailoverProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write(WWTUtil.GetCurrentConfigShare("DSSTOASTPNG", true));
        }
    }
}
