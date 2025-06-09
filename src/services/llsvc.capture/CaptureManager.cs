using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace LLSvc.Capture
{
    public enum CaptureMode
    {
        Window,
        FullScreen,
        Area,
        Flyout
    }

    internal static class CaptureManager
    {
        public static void CaptureWindow(bool includeShadow)
        {
            // TODO: integrate AeroShotCRE or Windows.Graphics.Capture
        }

        public static void CaptureFullScreen()
        {
            // TODO: capture across monitors
        }

        public static void CaptureArea(Rect area)
        {
            // TODO: capture custom region selected by user
        }

        public static void ShowFlyout()
        {
            var win = new CaptureWindow();
            win.Show();
        }

        public static void SaveToClipboard(Bitmap bmp)
        {
            using var ms = new System.IO.MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var img = System.Windows.Media.Imaging.BitmapFrame.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
            Clipboard.SetImage(img);
        }
    }
}
