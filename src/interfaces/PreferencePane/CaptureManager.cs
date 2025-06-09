using Microsoft.UI.Xaml;
using Windows.System;
using SleekySnip.Core;
using System.IO;
using Llsvc.KeyboardListener;

namespace PreferencePane;

public static class CaptureManager
{
    private static CaptureWindow? _window;
    private static KeyboardListener? _listener;

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
        if (_listener != null)
            return;

        var settingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "resources", "SleekySnip.props");
        var settings = SleekySnip.Core.SleekySnipSettingsSerializer.Load(settingsPath);

        VirtualKey key = VirtualKey.Snapshot;
        Enum.TryParse(settings.Hotkey, out key);

        _listener = new KeyboardListener();
        _listener.SetProcessCommand(id => ShowFlyout());
        _listener.SetHotkeyAction(win: false, ctrl: false, shift: false, alt: false, key: (byte)key, id: "capture");
        _listener.Start();

        Application.Current.Exit += OnAppExit;
    }

    private static void OnAppExit(object sender, object e)
    {
        _listener?.Dispose();
        _listener = null;
    }
}
