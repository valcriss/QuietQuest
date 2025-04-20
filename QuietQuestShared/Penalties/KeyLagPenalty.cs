using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuietQuestShared.Tools;

namespace QuietQuestShared.Penalties
{
    public sealed class KeyLagPenalty : IPenalty
    {
        public string Name => "Clavier qui Lag";

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            KeyboardInterceptor.SetLag(250);                // 250 ms
            try { await Task.Delay(duration, token); }
            finally { KeyboardInterceptor.SetLag(0); }
        }
    }
}
