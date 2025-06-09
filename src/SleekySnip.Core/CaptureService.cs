using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace SleekySnip.Core;

public static class CaptureService
{
    /// <summary>
    /// Captures the window specified by its handle. The capture is copied to the clipboard
    /// and optionally saved to <paramref name="savePath"/> if provided.
    /// </summary>
    /// <param name="hwnd">Handle to the window to capture.</param>
    /// <param name="savePath">Path to save the PNG file or <c>null</c> to only copy to the clipboard.</param>
    public static async Task CaptureWindowAsync(nint hwnd, string? savePath = null)
    {
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var item = CaptureHelper.CreateItemForWindow(windowId);
        await CaptureInternalAsync(item, savePath);
    }

    /// <summary>
    /// Captures the primary screen. The capture is copied to the clipboard and optionally saved.
    /// </summary>
    public static async Task CaptureScreenAsync(string? savePath = null)
    {
        var displayId = Win32Interop.GetDisplayIdFromWindow(nint.Zero);
        var item = CaptureHelper.CreateItemForMonitor(displayId);
        await CaptureInternalAsync(item, savePath);
    }

    /// <summary>
    /// Captures an arbitrary region of the screen.
    /// </summary>
    public static async Task CaptureRegionAsync(GraphicsCaptureItem item, string? savePath = null)
    {
        await CaptureInternalAsync(item, savePath);
    }

    private static async Task CaptureInternalAsync(GraphicsCaptureItem item, string? savePath)
    {
        var device = Direct3D11Helper.CreateDevice();
        using var framePool = Direct3D11CaptureFramePool.Create(device, DirectXPixelFormat.B8G8R8A8UIntNormalized, 1, item.Size);
        using var session = framePool.CreateCaptureSession(item);

        var tcs = new TaskCompletionSource<SoftwareBitmap>();
        void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            using var frame = sender.TryGetNextFrame();
            var task = SoftwareBitmap.CreateCopyFromSurfaceAsync(frame.Surface).AsTask();
            task.Wait();
            tcs.SetResult(task.Result);
        }

        framePool.FrameArrived += OnFrameArrived;
        session.StartCapture();

        var bitmap = await tcs.Task.ConfigureAwait(false);
        framePool.FrameArrived -= OnFrameArrived;
        session.Dispose();

        using var stream = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
        encoder.SetSoftwareBitmap(bitmap);
        await encoder.FlushAsync();
        stream.Seek(0);

        var dataPackage = new DataPackage();
        dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
        Clipboard.SetContent(dataPackage);

        if (!string.IsNullOrWhiteSpace(savePath))
        {
            using var fileStream = File.Create(savePath);
            stream.Seek(0);
            await stream.AsStreamForRead().CopyToAsync(fileStream);
        }
    }
}

internal static class CaptureHelper
{
    [DllImport("user32.dll")]
    private static extern nint GetDesktopWindow();

    public static GraphicsCaptureItem CreateItemForWindow(WindowId id)
    {
        return GraphicsCaptureItem.TryCreateFromWindowId(id);
    }

    public static GraphicsCaptureItem CreateItemForMonitor(DisplayId id)
    {
        return GraphicsCaptureItem.TryCreateFromDisplayId(id);
    }
}
