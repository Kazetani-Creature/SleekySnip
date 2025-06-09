using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LLSvc.Capture
{
    public partial class App : Application
    {
        private HwndSource? _source;
        private Window? _messageWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _messageWindow = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Opacity = 0
            };
            _messageWindow.SourceInitialized += (_, __) =>
            {
                var helper = new WindowInteropHelper(_messageWindow);
                _source = HwndSource.FromHwnd(helper.Handle);
                _source.AddHook(HwndHook);
                HotKeyManager.Register(helper.Handle);
            };
            _messageWindow.Show();
            _messageWindow.Hide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                var helper = new WindowInteropHelper(_messageWindow);
                HotKeyManager.Unregister(helper.Handle);
            }
            base.OnExit(e);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == HotKeyManager.HOTKEY_ID)
            {
                var captureWindow = new CaptureWindow();
                captureWindow.Show();
                handled = true;
            }
            return IntPtr.Zero;
        }
    }

    internal static class HotKeyManager
    {
        public const int HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_S = 0x53; // S key

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static bool Register(IntPtr handle)
        {
            return RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_S);
        }

        public static void Unregister(IntPtr handle)
        {
            UnregisterHotKey(handle, HOTKEY_ID);
        }
    }
}
