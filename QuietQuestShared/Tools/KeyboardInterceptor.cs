using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietQuestShared.Tools
{
    public static class KeyboardInterceptor
    {
        // ─── Public API ────────────────────────────────────────────────────────────
        public static bool Enabled => _hookId != IntPtr.Zero;
        public static void EnableInversion() => EnsureHook(invert: true);
        public static void DisableInversion() => RemoveHook();
        public static void SetLag(int ms) => _lagMs = Math.Max(0, ms);

        // ─── Internals ─────────────────────────────────────────────────────────────
        private static IntPtr _hookId;
        private static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static bool _invert;
        private static int _lagMs;

        private static readonly Dictionary<Keys, Keys> _mapAzerty = new()
    {
        { Keys.Z, Keys.S }, { Keys.S, Keys.Z },
        { Keys.Q, Keys.D }, { Keys.D, Keys.Q }
    };
        private static readonly Dictionary<Keys, Keys> _mapQwerty = new()
    {
        { Keys.W, Keys.S }, { Keys.S, Keys.W },
        { Keys.A, Keys.D }, { Keys.D, Keys.A }
    };

        private static void EnsureHook(bool invert)
        {
            if (_hookId != IntPtr.Zero) { _invert = invert; return; }

            _invert = invert;
            _hookId = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, _proc,
                                       GetModuleHandle(null), 0);
            if (_hookId == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
        }

        private static void RemoveHook()
        {
            if (_hookId != IntPtr.Zero) UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
            _invert = false;
            _lagMs = 0;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(_hookId, nCode, wParam, lParam);

            var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam)!;
            var key = (Keys)info.vkCode;

            // We only rewrite keydown events
            bool isDown = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;
            if (isDown && _invert && TryMap(ref key))
            {
                // swallow original
                if (_lagMs > 0)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(_lagMs);
                        SendInputKey(key, true);  // down
                        SendInputKey(key, false); // up
                    });
                }
                else
                {
                    SendInputKey(key, true);
                    SendInputKey(key, false);
                }
                return (IntPtr)1; // block
            }

            // Optional lag without inversion
            if (isDown && !_invert && _lagMs > 0)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(_lagMs);
                    SendInputKey(key, true);
                    SendInputKey(key, false);
                });
                return (IntPtr)1;
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static bool TryMap(ref Keys key)
        {
            if (_mapAzerty.TryGetValue(key, out var k) ||
                _mapQwerty.TryGetValue(key, out k))
            {
                key = k;
                return true;
            }
            return false;
        }

        private static void SendInputKey(Keys key, bool keyDown)
        {
            var input = new INPUT
            {
                type = 1, // KEYBOARD
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = (ushort)key,
                        dwFlags = keyDown ? 0u : 2u // KEYEVENTF_KEYUP
                    }
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>());
        }

        // ─── PInvoke & structs ─────────────────────────────────────────────────────
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private enum HookType : int { WH_KEYBOARD_LL = 13, WH_MOUSE_LL = 14 }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode; public uint scanCode;
            public uint flags; public uint time; public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk, wScan;
            public uint dwFlags, time;
            public UIntPtr dwExtraInfo;
        }
    }
}
