using System.Windows.Forms;
using QuietQuestShared.Audio;
using QuietQuestShared.Http;
using QuietQuestShared.Models;
using QuietQuestShared.Penalties;

namespace QuietQuestAgent
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var config = new Config();
            var audioMonitor = new AudioMonitor(() => config.Threshold);
            audioMonitor.ListAvailableDevices();
            var LastRaisedEvent = DateTime.Now;
            var currentAlertCount = 0;
            var penalties = new List<IPenalty>
            {
                new AnnoyingOverlayPenalty("Assets/loud.gif"),
                new AnnoyingMusicPenalty("Assets/baby.mp3"),
                new AnnoyingMusicPenalty("Assets/macarena.mp3"),
                new BrightnessPenalty(1),
                new EchoVoicePenalty(),
                new MuteAudioPenalty(),
                new MuteMicrophonePenalty(),
                new NotificationSoundPenalty("Assets/Notifications"),
                new TrollVoicePenalty(),
            };
            var penaltyManager = new PenaltyManager(penalties, TimeSpan.FromSeconds(10));
            audioMonitor.VolumeThresholdExceeded += () =>
            {
                if (!config.Active) return;
                if (penaltyManager.IsPenaltyRunning) return;
                if (DateTime.Now.Subtract(LastRaisedEvent).TotalSeconds < 30) return;
                LastRaisedEvent = DateTime.Now;
                currentAlertCount++;
                if(currentAlertCount >= config.AlertCount)
                {
                    currentAlertCount = 0;
                    VoiceAlert.Speak("Tu as fais trop de bruit, une pénalité va être appliquée");
                    Thread.Sleep(4000);
                    penaltyManager.Trigger();
                }
                else
                {
                    VoiceAlert.Speak($"Tu as fais trop de bruit");
                }
                
                
            };

            audioMonitor.Start();
            var admin = new AdminServer(audioMonitor, penaltyManager, config);
            admin.Start();

            Application.Run();
        }
    }
}