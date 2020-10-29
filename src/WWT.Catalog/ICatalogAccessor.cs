using System.Threading.Tasks;

namespace WWT.Catalog
{
    public interface ICatalogAccessor
    {
        Task<CatalogEntry> GetCatalogEntryAsync(string catalogEntryName);
    }
}
