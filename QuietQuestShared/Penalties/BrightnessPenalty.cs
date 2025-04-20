using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestShared.Penalties
{
    public sealed class BrightnessPenalty : IPenalty
    {
        public string Name => "Changement brutal de luminosité";

        private readonly byte _penaltyBrightnessLevel;

        public BrightnessPenalty(byte penaltyBrightnessLevel = 10) // très sombre ou très clair
        {
            _penaltyBrightnessLevel = penaltyBrightnessLevel;
        }

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            byte initialBrightness = GetCurrentBrightness();

            SetBrightness(_penaltyBrightnessLevel);

            try
            {
                await Task.Delay(duration, token);
            }
            catch (TaskCanceledException) { }
            finally
            {
                SetBrightness(initialBrightness); // Restaurer la luminosité initiale
            }
        }

        private void SetBrightness(byte brightness)
        {
            using var mclass = new ManagementClass("WmiMonitorBrightnessMethods");
            mclass.Scope = new ManagementScope(@"\\.\root\wmi");
            foreach (ManagementObject instance in mclass.GetInstances())
            {
                instance.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, brightness });
            }
        }

        private byte GetCurrentBrightness()
        {
            using var searcher = new ManagementObjectSearcher(@"\\.\root\wmi", "SELECT CurrentBrightness FROM WmiMonitorBrightness");
            using var results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                return (byte)result["CurrentBrightness"];
            }

            return 50; // valeur par défaut en cas d'échec
        }
    }
}
