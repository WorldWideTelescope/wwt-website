using WWT.Catalog;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Catalog2.aspx")]
    public class Catalog2Provider : CatalogProvider
    {
        public Catalog2Provider(ICatalogAccessor catalog)
            : base(catalog, true)
        {
        }

        public override string ContentType => ContentTypes.Text;
    }
}