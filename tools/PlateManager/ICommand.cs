using System.Threading;
using System.Threading.Tasks;

namespace PlateManager
{
    internal interface ICommand
    {
        Task RunAsync(CancellationToken token);
    }
}
