using System;
using System.IO;

namespace WWT.Catalog
{
    public class CatalogEntry
    {
        public DateTime LastModified { get; set; }

        public Stream Contents { get; set; }
    }
}
