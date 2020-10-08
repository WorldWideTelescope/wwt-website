using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PlateManager
{
    internal interface IWorkItemGenerator
    {
        IEnumerable<Func<int, int, CancellationToken, Task>> GenerateWorkItems(string plateFile, string baseUrl, string container);
    }
}