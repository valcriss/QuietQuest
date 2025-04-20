using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuietQuestShared.Tools;

namespace QuietQuestShared.Penalties
{
    public sealed class MouseDriftPenalty : IPenalty
    {
        public string Name => "Souris folle";

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            MouseInterceptor.StartRandomDrift();
            try { await Task.Delay(duration, token); }
            finally { MouseInterceptor.StopRandomDrift(); }
        }
    }
}
