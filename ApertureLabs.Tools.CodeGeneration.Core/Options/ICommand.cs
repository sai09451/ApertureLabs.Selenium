using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    public interface ICommand
    {
        Task ExecuteAsync(
            IProgress<double> progress,
            CancellationToken cancellationToken);
    }
}
