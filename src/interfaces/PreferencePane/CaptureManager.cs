using Microsoft.UI.Xaml;
using Windows.System;
using SleekySnip.Core;
using System.IO;
using System.Runtime.InteropServices;
using Llsvc.KeyboardListener;

namespace PreferencePane;

public static class CaptureManager
{
    private static CaptureWindow? _window;
    private static KeyboardListener? _listener;
    private static SleekySnipSettings? _settings;

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
        _settings = SleekySnip.Core.SleekySnipSettingsSerializer.Load(settingsPath);

        _listener = new KeyboardListener();
        _listener.SetProcessCommand(ProcessCommand);

        RegisterHotkey(_settings.ScreenHotkey, "screen");
        RegisterHotkey(_settings.WindowHotkey, "window");
        RegisterHotkey(_settings.RegionHotkey, "region");

        _listener.Start();

        Application.Current.Exit += OnAppExit;
    }

    private static void OnAppExit(object sender, object e)
    {
        _listener?.Dispose();
        _listener = null;
    }

    private static void RegisterHotkey(string text, string id)
    {
        if (_listener == null)
            return;

        ParseHotkey(text, out bool win, out bool ctrl, out bool shift, out bool alt, out VirtualKey key);
        _listener.SetHotkeyAction(win, ctrl, shift, alt, (byte)key, id);
    }

    private static void ProcessCommand(string id)
    {
        if (_settings == null)
            return;

        string? file = string.IsNullOrWhiteSpace(_settings.OutputFolder)
            ? null
            : Path.Combine(_settings.OutputFolder, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        switch (id)
        {
            case "screen":
                _ = CaptureService.CaptureScreenAsync(file);
                break;
            case "window":
                nint hwnd = GetForegroundWindow();
                if (hwnd != nint.Zero)
                    _ = CaptureService.CaptureWindowAsync(hwnd, file);
                break;
            case "region":
                ShowFlyout();
                break;
        }
    }

    private static void ParseHotkey(string text, out bool win, out bool ctrl, out bool shift, out bool alt, out VirtualKey key)
    {
        win = ctrl = shift = alt = false;
        key = VirtualKey.None;
        foreach (var token in text.Split('+', StringSplitOptions.RemoveEmptyEntries))
        {
            var part = token.Trim();
            if (part.Equals("win", StringComparison.OrdinalIgnoreCase)) win = true;
            else if (part.Equals("ctrl", StringComparison.OrdinalIgnoreCase) || part.Equals("control", StringComparison.OrdinalIgnoreCase)) ctrl = true;
            else if (part.Equals("shift", StringComparison.OrdinalIgnoreCase)) shift = true;
            else if (part.Equals("alt", StringComparison.OrdinalIgnoreCase)) alt = true;
            else if (Enum.TryParse(part, true, out VirtualKey parsed)) key = parsed;
        }
        if (key == VirtualKey.None)
            key = VirtualKey.Snapshot;
    }

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();
}
