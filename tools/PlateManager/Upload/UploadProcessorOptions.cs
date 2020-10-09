using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlateManager
{
    internal class UploadProcessorOptions : BaseOptions, IServiceRegistration
    {
        public const int DefaultUploaderCount = 50;

        public IEnumerable<FileInfo> File { get; set; } = Enumerable.Empty<FileInfo>();

        public bool UsePlate2Format { get; set; }
        
        public int UploaderCount { get; set; } = DefaultUploaderCount;

        public string BaseUrl { get; set; }

        public IEnumerable<string> Files => File.Select(f => f.FullName);

        void IServiceRegistration.AddServices(IServiceCollection services)
        {
            if (UsePlate2Format)
            {
                services.AddTransient<IWorkItemGenerator, PlateFile2WorkItemGenerator>();
            }
            else
            {
                services.AddTransient<IWorkItemGenerator, PlateFileWorkItemGenerator>();
            }

            services.AddSingleton<ICommand, UploadProcessor>();
        }
    }
}
