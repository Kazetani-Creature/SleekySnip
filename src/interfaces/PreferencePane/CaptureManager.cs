using Microsoft.UI.Xaml;
using System.Windows.Forms;
using Windows.System;
using SleekySnip.Core;
using System.IO;

namespace PreferencePane;

public static class CaptureManager
{
    private static CaptureWindow? _window;
    private static HotKeyRegistrar? _registrar;
    private static MessageWindow? _messageWindow;

    private const int WM_HOTKEY = 0x0312;

    private sealed class MessageWindow : NativeWindow
    {
        public MessageWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                ShowFlyout();
            }

            base.WndProc(ref m);
        }
    }

    public static void ShowFlyout()
    {
        if (_window == null)
        {
            _window = new CaptureWindow();
            _window.Closed += (_, _) => _window = null;
        }

        _window.Activate();
    }

    public static void Initialize()
    {
        if (_messageWindow != null)
            return;

        _messageWindow = new MessageWindow();

        var settingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "resources", "SleekySnip.props");
        var settings = SleekySnip.Core.SleekySnipSettingsSerializer.Load(settingsPath);

        VirtualKey key = VirtualKey.Snapshot;
        Enum.TryParse(settings.Hotkey, out key);

        _registrar = new SleekySnip.Core.HotKeyRegistrar(_messageWindow.Handle);
        _registrar.TryRegister(0, (uint)key);

        Application.Current.Exit += OnAppExit;
    }

    private static void OnAppExit(object sender, object e)
    {
        _registrar?.Dispose();
        _registrar = null;
        _messageWindow?.DestroyHandle();
        _messageWindow = null;
    }
}
