using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace QuietQuestShared.Penalties
{
    public sealed class EchoVoicePenalty : IPenalty
    {
        public string Name => "Écho voix";

        private readonly int _delayMs;

        public EchoVoicePenalty(int delayMs = 1500) // délai par défaut : 1,5 seconde
        {
            _delayMs = delayMs;
        }

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            var waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1),
                BufferMilliseconds = 50
            };

            var bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true
            };

            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(bufferedWaveProvider);

            waveIn.DataAvailable += (_, args) =>
            {
                // Ajoute les données capturées au buffer
                bufferedWaveProvider.AddSamples(args.Buffer, 0, args.BytesRecorded);
            };

            waveIn.StartRecording();

            // Attente initiale pour créer le délai d'écho
            await Task.Delay(_delayMs, token).ContinueWith(_ => { });

            outputDevice.Play();

            try
            {
                await Task.Delay(duration, token);
            }
            catch (TaskCanceledException)
            {
                // Ignorer l'annulation
            }
            finally
            {
                waveIn.StopRecording();
                outputDevice.Stop();
            }
        }
    }
}
