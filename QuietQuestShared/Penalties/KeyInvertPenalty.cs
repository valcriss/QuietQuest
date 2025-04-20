using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuietQuestShared.Tools;

namespace QuietQuestShared.Penalties
{
    public sealed class KeyInvertPenalty : IPenalty
    {
        public string Name => "Inversion de clavier";

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            KeyboardInterceptor.EnableInversion();          // à toi d’implémenter
            await Task.Delay(duration, token)
                      .ContinueWith(_ => { });              // ignore Cancelled
            KeyboardInterceptor.DisableInversion();
        }
    }
}
