namespace WWT.Providers
{
    public interface IWwtContext
    {
        IRequest Request { get; }

        IResponse Response { get; }

        string MachineName { get; }

        string MapPath(string path);
    }
}
