using Microsoft.Extensions.DependencyInjection;

namespace PlateManager.List
{
    class ListCommandOptions : BaseOptions, IServiceRegistration
    {
        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<ICommand, ListCommand>();
        }
    }
}
