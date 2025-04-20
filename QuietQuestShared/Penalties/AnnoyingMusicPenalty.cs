using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace QuietQuestShared.Penalties
{
    public sealed class AnnoyingMusicPenalty : IPenalty
    {
        public string Name => "Musique agaçante";

        private readonly string _audioFilePath;

        public AnnoyingMusicPenalty(string audioFilePath)
        {
            _audioFilePath = audioFilePath;
        }

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            using var audioFile = new AudioFileReader(_audioFilePath);
            using var outputDevice = new WaveOutEvent();

            outputDevice.Init(audioFile);
            outputDevice.Play();

            try
            {
                await Task.Delay(duration, token);
            }
            catch (TaskCanceledException)
            {
                // Ignorer si annulation
            }
            finally
            {
                outputDevice.Stop();
            }
        }
    }
}
