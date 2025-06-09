using System;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

namespace SleekySnip.Core;

internal static class Direct3D11Helper
{
    public static IDirect3DDevice CreateDevice()
    {
        var dxgiDevice = CreateDxgiDevice();
        var device = Direct3D11Helper.CreateDirect3DDeviceFromDxgiDevice(dxgiDevice);
        return device;
    }

    private static SharpDX.DXGI.Device CreateDxgiDevice()
    {
        return new SharpDX.DXGI.Device(SharpDX.Direct3D.DriverType.Hardware,
            SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport);
    }
}
