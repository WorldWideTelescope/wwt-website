using WWT.Catalog;

namespace WWT.Providers
{
    public class Catalog2Provider : CatalogProvider
    {
        public Catalog2Provider(FilePathOptions options, ICatalogAccessor catalog) : base(options, catalog, true)
        {
        }
    }
}