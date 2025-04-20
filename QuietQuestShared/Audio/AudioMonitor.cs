using System.Diagnostics;
using NAudio.Wave;

namespace QuietQuestShared.Audio
{
    public class AudioMonitor
    {
        private readonly WaveInEvent _waveIn;
        private readonly Func<int> _thresholdProvider;

        public event Action? VolumeThresholdExceeded;

        public AudioMonitor(Func<int> thresholdProvider, int deviceNumber = 0)
        {
            _thresholdProvider = thresholdProvider;

            _waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new WaveFormat(44100, 1),
                BufferMilliseconds = 100
            };
            _waveIn.DataAvailable += OnDataAvailable;
        }

        public void Start() => _waveIn.StartRecording();
        public void Stop() => _waveIn.StopRecording();

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            double rms = 0;
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
                float sampleF = sample / 32768f;
                rms += sampleF * sampleF;
            }
            rms = Math.Sqrt(rms / (e.BytesRecorded / 2));
            int volume = (int)(rms * 100);
            Debug.WriteLine($"Volume: {volume}");
            int threshold = _thresholdProvider();
            if (volume > threshold)
                VolumeThresholdExceeded?.Invoke();
        }

        public void ListAvailableDevices()
        {
            int waveInDevices = WaveInEvent.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(waveInDevice);
                Debug.WriteLine($"Device {waveInDevice}: {deviceInfo.ProductName}");
            }
        }
    }
}
