using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestShared.Penalties
{
    public interface IPenalty
    {
        string Name { get; }

        /// <summary>Doit appliquer l’effet, attendre la durée, puis restaurer l’état.</summary>
        Task ExecuteAsync(TimeSpan duration, CancellationToken token);
    }
}
