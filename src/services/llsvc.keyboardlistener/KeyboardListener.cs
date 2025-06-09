using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Llsvc.KeyboardListener;

public class KeyboardListener : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)]
        public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const int INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public record struct Hotkey(bool Win, bool Ctrl, bool Shift, bool Alt, byte Key);

    public delegate void ProcessCommand(string id);

    private readonly SortedDictionary<Hotkey, string> _hotkeyMap = new(new HotkeyComparer());
    private readonly LowLevelProc _proc;
    private IntPtr _hook;
    private ProcessCommand? _processCommand;

    public KeyboardListener()
    {
        _proc = HookCallback;
    }

    public void Start()
    {
        if (_hook == IntPtr.Zero)
        {
            _hook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);
            if (_hook == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    public void Stop()
    {
        if (_hook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hook);
            _hook = IntPtr.Zero;
        }
    }

    public void SetHotkeyAction(bool win, bool ctrl, bool shift, bool alt, byte key, string id)
    {
        var hotkey = new Hotkey(win, ctrl, shift, alt, key);
        _hotkeyMap[hotkey] = id;
    }

    public void ClearHotkey(string id)
    {
        foreach (var kv in new List<KeyValuePair<Hotkey, string>>(_hotkeyMap))
        {
            if (kv.Value == id)
            {
                _hotkeyMap.Remove(kv.Key);
            }
        }
    }

    public void ClearHotkeys() => _hotkeyMap.Clear();

    public void SetProcessCommand(ProcessCommand processCommand)
    {
        _processCommand = processCommand;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var hotkey = new Hotkey(
                (GetAsyncKeyState(0x5B) & 0x8000) != 0 || (GetAsyncKeyState(0x5C) & 0x8000) != 0,
                (GetAsyncKeyState(0x11) & 0x8000) != 0,
                (GetAsyncKeyState(0x10) & 0x8000) != 0,
                (GetAsyncKeyState(0x12) & 0x8000) != 0,
                (byte)info.vkCode);

            if (!hotkey.Equals(default(Hotkey)) && _hotkeyMap.TryGetValue(hotkey, out string? actionId))
            {
                _processCommand?.Invoke(actionId);

                INPUT[] input = new INPUT[1];
                input[0].type = INPUT_KEYBOARD;
                input[0].u.ki.wVk = 0xFF;
                input[0].u.ki.dwFlags = KEYEVENTF_KEYUP;
                SendInput(1, input, Marshal.SizeOf<INPUT>());
                return (IntPtr)1;
            }
        }

        return CallNextHookEx(_hook, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        Stop();
    }

    private class HotkeyComparer : IComparer<Hotkey>
    {
        public int Compare(Hotkey x, Hotkey y)
        {
            int result = x.Key.CompareTo(y.Key);
            if (result != 0) return result;
            result = BoolCompare(x.Win, y.Win);
            if (result != 0) return result;
            result = BoolCompare(x.Ctrl, y.Ctrl);
            if (result != 0) return result;
            result = BoolCompare(x.Shift, y.Shift);
            if (result != 0) return result;
            return BoolCompare(x.Alt, y.Alt);
        }

        private static int BoolCompare(bool a, bool b) => a == b ? 0 : a ? 1 : -1;
    }
}
