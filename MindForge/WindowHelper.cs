using System;
using System.Runtime.InteropServices;

public static class WindowHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    public static void MaximizeWindow(string windowTitle)
    {
        IntPtr hWnd = FindWindow(null, windowTitle);

        if (hWnd != IntPtr.Zero)
        {
            ShowWindow(hWnd, 1);
            SetForegroundWindow(hWnd);
        }
    }
}