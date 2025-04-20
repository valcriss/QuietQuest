using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace QuietQuestShared.Penalties
{
    public sealed class MuteMicrophonePenalty : IPenalty
    {
        public string Name => "Couper le micro";

        public async Task ExecuteAsync(TimeSpan duration, CancellationToken token)
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

            device.AudioEndpointVolume.Mute = true;

            try
            {
                var endTime = DateTime.Now + duration;
                while (DateTime.Now < endTime && !token.IsCancellationRequested)
                {
                    if (!device.AudioEndpointVolume.Mute)
                        device.AudioEndpointVolume.Mute = true;

                    await Task.Delay(500, token);
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                device.AudioEndpointVolume.Mute = false;
            }
        }
    }
}
