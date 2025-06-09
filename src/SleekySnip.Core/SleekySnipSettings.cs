namespace SleekySnip.Core;

public class SleekySnipSettings
{
    public string ScreenHotkey { get; set; } = "PrintScreen";
    public string WindowHotkey { get; set; } = "Alt+PrintScreen";
    public string RegionHotkey { get; set; } = "Win+Shift+S";

    public string CaptureMode { get; set; } = "Region";
    public string OutputFolder { get; set; } = string.Empty;
}
