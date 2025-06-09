using Microsoft.UI.Xaml;

namespace PreferencePane;

public static class CaptureManager
{
    private static CaptureWindow? _window;

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
        // TODO: register global hotkey and call ShowFlyout when triggered
    }
}
