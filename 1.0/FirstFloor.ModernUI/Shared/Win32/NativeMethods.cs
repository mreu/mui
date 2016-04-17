namespace FirstFloor.ModernUI.Win32
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native methods class.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// The S_OK (const). Value: 0.
        /// </summary>
        public const int S_OK = 0;

        /// <summary>
        /// The WM_DPICHANGED (const). Value: 0x02E0.
        /// </summary>
        public const int WM_DPICHANGED = 0x02E0;

        /// <summary>
        /// The MONITOR_DEFAULTTONEAREST (const). Value: 0x00000002.
        /// </summary>
        public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        /// <summary>
        /// The GetProcessDpiAwareness api call.
        /// </summary>
        /// <param name="hprocess">The hprocess.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("Shcore.dll")]
        public static extern int GetProcessDpiAwareness(IntPtr hprocess, out ProcessDpiAwareness value);

        /// <summary>
        /// The SetProcessDpiAwareness api call.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("Shcore.dll")]
        public static extern int SetProcessDpiAwareness(ProcessDpiAwareness value);

        /// <summary>
        /// The IsProcessDPIAware api call.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("user32.dll")]
        public static extern bool IsProcessDPIAware();

        /// <summary>
        /// The SetProcessDPIAware api call.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("user32.dll")]
        public static extern int SetProcessDPIAware();

        /// <summary>
        /// The GetDpiForMonitor api call.
        /// </summary>
        /// <param name="hMonitor">The hMonitor.</param>
        /// <param name="dpiType">The dpiType.</param>
        /// <param name="xDpi">The xDpi.</param>
        /// <param name="yDpi">The yDpi.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("shcore.dll")]
        public static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, ref uint xDpi, ref uint yDpi);

        /// <summary>
        /// The MonitorFromWindow api call.
        /// </summary>
        /// <param name="hwnd">The hwnd.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>The <see cref="IntPtr"/>.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flag);
    }
}
