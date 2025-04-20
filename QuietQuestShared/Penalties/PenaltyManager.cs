using System.Diagnostics;
using System.Timers;
using QuietQuestShared.Audio;

namespace QuietQuestShared.Penalties
{
    public class PenaltyManager
    {
        private readonly IReadOnlyList<IPenalty> _penalties;
        private readonly TimeSpan _duration;
        private readonly Random _rng = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool _isRunning = false;
        private CancellationTokenSource? _cts;
        public bool IsPenaltyRunning => _isRunning;
        public DateTime? LastTriggered { get; private set; }
        public string? LastPenaltyName { get; private set; }

        public PenaltyManager(IEnumerable<IPenalty> penalties, TimeSpan duration)
        {
            _penalties = penalties.ToList().AsReadOnly();
            _duration = duration;
        }

        public async void Trigger()
        {
            // empêche un chevauchement de pénalités
            if (!await _semaphore.WaitAsync(0)) return;

            try
            {
                _isRunning=true;
                // choisit une pénalité au hasard
                IPenalty p = _penalties[_rng.Next(_penalties.Count)];
                LastPenaltyName = p.Name;
                LastTriggered = DateTime.Now;

                _cts = new CancellationTokenSource();
                VoiceAlert.Speak($"Début de la pénalité : {p.Name}");
                Thread.Sleep(4000);
                await p.ExecuteAsync(_duration, _cts.Token);
                VoiceAlert.Speak($"Fin de la pénalité : {p.Name}");
            }
            catch (OperationCanceledException) { /* ignore */ }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                _semaphore.Release();
                _isRunning = false;
            }
        }

        public void CancelCurrent() => _cts?.Cancel();
    }
}
