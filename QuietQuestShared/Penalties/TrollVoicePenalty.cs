using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestShared.Penalties
{
    public sealed class TrollVoicePenalty : IPenalty
    {
        public string Name => "Message vocal troll";

        private readonly string[] _messages = new[]
        {
            "As-tu essayé de ne pas crier pour mieux jouer ?",
            "Tu cries tellement fort que même le voisin a perdu sa partie !"
        };

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            var synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);

            var rnd = new Random();
            var endTime = DateTime.Now + duration;

            while (DateTime.Now < endTime && !token.IsCancellationRequested)
            {
                string msg = _messages[rnd.Next(_messages.Length)];
                synth.SpeakAsync(msg);
                await Task.Delay(4000, token); // Attend 4 secondes entre chaque message
            }

            synth.Dispose();
        }
    }
}
