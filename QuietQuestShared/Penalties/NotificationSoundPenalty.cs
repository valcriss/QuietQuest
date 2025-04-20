using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace QuietQuestShared.Penalties
{
    public sealed class NotificationSoundPenalty : IPenalty
    {
        public string Name => "Sons perturbants";

        private readonly string[] _soundFiles;
        private readonly Random _rng = new();

        public NotificationSoundPenalty(string soundFolder)
        {
            _soundFiles = Directory.GetFiles(soundFolder, "*.mp3");
        }

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            var endTime = DateTime.Now + duration;

            while (DateTime.Now < endTime && !token.IsCancellationRequested)
            {
                string soundFile = _soundFiles[_rng.Next(_soundFiles.Length)];
                using var audioFile = new AudioFileReader(soundFile);
                using var outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Attends entre 0.5 et 2 secondes entre chaque notification
                int delayMs = _rng.Next(500, 2000);
                await Task.Delay(delayMs, token).ContinueWith(_ => { });

                outputDevice.Stop();
            }
        }
    }
}
