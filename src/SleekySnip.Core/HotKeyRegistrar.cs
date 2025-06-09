using System;
using System.Runtime.InteropServices;

namespace SleekySnip.Core
{
    /// <summary>
    /// Provides helpers to register global hotkeys and check for failures.
    /// </summary>
    public sealed class HotKeyRegistrar : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly IntPtr _windowHandle;
        private int _hotKeyId;

        public HotKeyRegistrar(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        /// <summary>
        /// Attempts to register a hotkey multiple times. Logs errors to stderr.
        /// </summary>
        /// <param name="modifiers">Modifier keys.</param>
        /// <param name="key">Virtual key code.</param>
        /// <param name="attempts">Number of attempts.</param>
        /// <returns>True if registration succeeds.</returns>
        public bool TryRegister(uint modifiers, uint key, int attempts = 3)
        {
            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                if (RegisterHotKey(_windowHandle, ++_hotKeyId, modifiers, key))
                    return true;

                int error = Marshal.GetLastWin32Error();
                Console.Error.WriteLine($"Failed to register hotkey (attempt {attempt}). Error: {error}");
            }

            Console.Error.WriteLine($"Unable to register hotkey after {attempts} attempts. Shutting down.");
            return false;
        }

        public void Dispose()
        {
            for (int id = 1; id <= _hotKeyId; id++)
                UnregisterHotKey(_windowHandle, id);
        }
    }
}
