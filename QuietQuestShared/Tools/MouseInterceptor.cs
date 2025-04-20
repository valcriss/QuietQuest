using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestShared.Tools
{
    public static class MouseInterceptor
    {
        private static CancellationTokenSource? _cts;
        private static readonly Random _rng = new();

        public static bool IsDrifting => _cts is { IsCancellationRequested: false };

        public static void StartRandomDrift(int amplitude = 6, int intervalMs = 35)
        {
            if (IsDrifting) return;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    GetCursorPos(out var p);
                    int dx = _rng.Next(-amplitude, amplitude + 1);
                    int dy = _rng.Next(-amplitude, amplitude + 1);
                    SetCursorPos(p.X + dx, p.Y + dy);
                    await Task.Delay(intervalMs, token).ContinueWith(_ => { });
                }
            }, token);
        }

        public static void StopRandomDrift() => _cts?.Cancel();

        // ─── PInvoke ───────────────────────────────────────────────────────────────
        [DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }
    }
}
