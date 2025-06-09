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
                var helper = new WindowInteropHelper(_messageWindow!);
                HotKeyManager.Unregister(helper.Handle);
            }
            base.OnExit(e);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                var id = (HotkeyId)wParam.ToInt32();
                switch (id)
                {
                    case HotkeyId.Window:
                        CaptureManager.CaptureWindow(includeShadow: true);
                        break;
                    case HotkeyId.FullScreen:
                        CaptureManager.CaptureFullScreen();
                        break;
                    case HotkeyId.Area:
                        CaptureManager.CaptureArea(new Rect());
                        break;
                    case HotkeyId.Flyout:
                        CaptureManager.ShowFlyout();
                        break;
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }

    internal enum HotkeyId
    {
        Window = 1,
        FullScreen,
        Area,
        Flyout
    }

    internal static class HotKeyManager
    {
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_NUMPAD4 = 0x64;
        private const uint VK_NUMPAD1 = 0x61;
        private const uint VK_3 = 0x33;
        private const uint VK_NUMPAD5 = 0x65;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void Register(IntPtr handle)
        {
            RegisterHotKey(handle, (int)HotkeyId.Window, MOD_ALT | MOD_SHIFT, VK_NUMPAD4);
            RegisterHotKey(handle, (int)HotkeyId.FullScreen, MOD_ALT | MOD_SHIFT, VK_NUMPAD1);
            RegisterHotKey(handle, (int)HotkeyId.Area, MOD_ALT | MOD_SHIFT, VK_3);
            RegisterHotKey(handle, (int)HotkeyId.Flyout, MOD_ALT | MOD_SHIFT, VK_NUMPAD5);
        }

        public static void Unregister(IntPtr handle)
        {
            foreach (HotkeyId id in Enum.GetValues(typeof(HotkeyId)))
            {
                UnregisterHotKey(handle, (int)id);
            }
        }
    }
}
