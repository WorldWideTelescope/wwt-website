using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;

namespace PlateManager.Download
{
    class DownloadCommandOptions : BaseOptions, IServiceRegistration
    {
        public IEnumerable<string> Plate { get; set; }

        public DirectoryInfo Output { get; set; }

        public int Levels { get; set; }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<ICommand, DownloadCommand>();
        }
    }
}
