using Microsoft.Extensions.DependencyInjection;

namespace PlateManager
{
    internal interface IServiceRegistration
    {
        void AddServices(IServiceCollection services);
    }
}
