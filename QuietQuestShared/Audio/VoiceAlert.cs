using System.Diagnostics;
using System.Speech.Synthesis;

namespace QuietQuestShared.Audio
{
    public static class VoiceAlert
    {
        private static SpeechSynthesizer synth = new();

        public static void Speak(string message)
        {
            synth.SelectVoiceByHints(VoiceGender.Female);
            synth.SpeakAsync(message);
            Debug.WriteLine($"Speaking: {message}");
        }
    }
}
