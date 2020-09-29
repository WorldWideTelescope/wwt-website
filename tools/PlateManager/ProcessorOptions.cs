using System.Collections.Generic;
using System.Linq;

namespace PlateManager
{
    internal class ProcessorOptions
    {
        public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

        public string BaseUrl { get; set; } = "devurl";

        public int FileProcessorCount { get; set; } = 5;

        public int UploadingCount { get; set; } = 50;

        public bool UsePlate2Format { get; set; }
    }
}
