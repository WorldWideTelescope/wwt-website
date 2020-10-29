namespace WWT.Providers
{
    public interface IWwtContext
    {
        ICache Cache { get; }

        IRequest Request { get; }

        IResponse Response { get; }

        string MachineName { get; }

        string MapPath(params string[] path);
    }
}
