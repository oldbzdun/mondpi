using System;
using System.Runtime.InteropServices;

class Program
{
    enum Monitor_DPI_Type
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default = MDT_Effective_DPI
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor,
        ref Rect lprcMonitor, IntPtr dwData);

    [DllImport("User32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
        MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("Shcore.dll")]
    static extern int GetDpiForMonitor(IntPtr hMonitor, Monitor_DPI_Type dpiType,
        out uint dpiX, out uint dpiY);

    [DllImport("Shcore.dll")]
    private static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

    enum PROCESS_DPI_AWARENESS
    {
        Process_DPI_Unaware = 0,
        Process_System_DPI_Aware = 1,
        Process_Per_Monitor_DPI_Aware = 2
    }

    static void Main()
    {
        SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
            {
                if (GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Effective_DPI, out uint dpiX, out uint dpiY) == 0)
                {
                    float scale = dpiX / 96.0f * 100;
                    Console.WriteLine($"Монитор {hMonitor}: Масштаб {scale}% (DPI: {dpiX})");
                }
                else
                {
                    Console.WriteLine($"Не удалось получить DPI для монитора {hMonitor}");
                }
                return true;
            }, IntPtr.Zero);

        Console.ReadLine();
    }
}
