using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SleekySnip.Core;

public static class CaptureManager
{
    public static void CaptureScreen(SleekySnipSettings settings)
    {
        using var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

        // copy to clipboard
        Clipboard.SetImage(bitmap);

        // save to file if output folder configured
        if (!string.IsNullOrWhiteSpace(settings.OutputFolder))
        {
            Directory.CreateDirectory(settings.OutputFolder);
            var file = Path.Combine(settings.OutputFolder, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            bitmap.Save(file, ImageFormat.Png);
        }
    }
}
