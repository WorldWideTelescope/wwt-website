using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace PlateManager.List
{
    class ListCommand : ICommand
    {
        private readonly IPlateTilePyramid _plateTiles;

        public ListCommand(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public async Task RunAsync(CancellationToken token)
        {
            await foreach (var item in _plateTiles.GetPlateNames(token))
            {
                Console.WriteLine(item);
            }
        }
    }
}
