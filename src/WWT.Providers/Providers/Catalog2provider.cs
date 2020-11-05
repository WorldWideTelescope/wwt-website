using WWT.Catalog;

namespace WWT.Providers
{
    public class Catalog2Provider : CatalogProvider
    {
        public Catalog2Provider(ICatalogAccessor catalog)
            : base(catalog, true)
        {
        }

        public override string ContentType => ContentTypes.Text;
    }
}