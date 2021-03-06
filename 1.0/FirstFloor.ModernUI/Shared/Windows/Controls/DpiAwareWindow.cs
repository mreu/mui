﻿namespace FirstFloor.ModernUI.Windows.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using FirstFloor.ModernUI.Win32;

    using Microsoft.Win32;

    /// <summary>
    /// A window instance that is capable of per-monitor DPI awareness when supported.
    /// </summary>
    public abstract class DpiAwareWindow
        : Window
    {
        /// <summary>
        /// Occurs when the system or monitor DPI for this window has changed.
        /// </summary>
        public event EventHandler DpiChanged;

        /// <summary>
        /// The source.
        /// </summary>
        private HwndSource source;

        /// <summary>
        /// The dpi info.
        /// </summary>
        private DpiInformation dpiInfo;

        /// <summary>
        /// The is per monitor dpi aware (readonly).
        /// </summary>
        private readonly bool isPerMonitorDpiAware;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpiAwareWindow"/> class.
        /// </summary>
        public DpiAwareWindow()
        {
            SourceInitialized += OnSourceInitialized;

            // WM_DPICHANGED is not send when window is minimized, do listen to global display setting changes
            SystemEvents.DisplaySettingsChanged += OnSystemEventsDisplaySettingsChanged;

            // try to set per-monitor dpi awareness, before the window is displayed
            isPerMonitorDpiAware = ModernUIHelper.TrySetPerMonitorDpiAware();
        }

        /// <summary>
        /// Gets the DPI information for this window instance.
        /// </summary>
        /// <remarks>
        /// DPI information is available after a window handle has been created.
        /// </remarks>
        public DpiInformation DpiInformation => dpiInfo;

        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // detach global event handlers
            SystemEvents.DisplaySettingsChanged -= OnSystemEventsDisplaySettingsChanged;
        }

        /// <summary>
        /// Raises the system events display settings changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSystemEventsDisplaySettingsChanged(object sender, EventArgs e)
        {
            if (source != null && WindowState == WindowState.Minimized)
            {
                RefreshMonitorDpi();
            }
        }

        /// <summary>
        /// Raises the source initialized event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            source = (HwndSource)PresentationSource.FromVisual(this);

            // calculate the DPI used by WPF; this is the same as the system DPI
            var matrix = source.CompositionTarget.TransformToDevice;

            dpiInfo = new DpiInformation(96D * matrix.M11, 96D * matrix.M22);

            if (isPerMonitorDpiAware)
            {
                source.AddHook(WndProc);

                RefreshMonitorDpi();
            }
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="hwnd">The hwnd.</param>
        /// <param name="msg">The msg.</param>
        /// <param name="wParam">The wParam.</param>
        /// <param name="lParam">The lParam.</param>
        /// <param name="handled">The handled.</param>
        /// <returns>The <see cref="IntPtr"/>.</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_DPICHANGED)
            {
                // Marshal the value in the lParam into a Rect.
                var newDisplayRect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                // Set the Window's position & size.
                var matrix = source.CompositionTarget.TransformFromDevice;
                var ul = matrix.Transform(new Vector(newDisplayRect.left, newDisplayRect.top));
                var hw = matrix.Transform(new Vector(newDisplayRect.right - newDisplayRect.left, newDisplayRect.bottom - newDisplayRect.top));
                Left = ul.X;
                Top = ul.Y;
                UpdateWindowSize(hw.X, hw.Y);

                // Remember the current DPI settings.
                var oldDpiX = dpiInfo.MonitorDpiX;
                var oldDpiY = dpiInfo.MonitorDpiY;

                // Get the new DPI settings from wParam
                var dpiX = (double)(wParam.ToInt32() >> 16);
                var dpiY = (double)(wParam.ToInt32() & 0x0000FFFF);

                if (oldDpiX != dpiX || oldDpiY != dpiY)
                {
                    dpiInfo.UpdateMonitorDpi(dpiX, dpiY);

                    // update layout scale
                    UpdateLayoutTransform();

                    // raise DpiChanged event
                    OnDpiChanged(EventArgs.Empty);
                }

                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Update layout transform.
        /// </summary>
        private void UpdateLayoutTransform()
        {
            if (isPerMonitorDpiAware)
            {
                var root = (FrameworkElement)GetVisualChild(0);
                if (root != null)
                {
                    if (dpiInfo.ScaleX != 1 || dpiInfo.ScaleY != 1)
                    {
                        root.LayoutTransform = new ScaleTransform(dpiInfo.ScaleX, dpiInfo.ScaleY);
                    }
                    else
                    {
                        root.LayoutTransform = null;
                    }
                }
            }
        }

        /// <summary>
        /// Update window size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void UpdateWindowSize(double width, double height)
        {
            // determine relative scalex and scaley
            var relScaleX = width / Width;
            var relScaleY = height / Height;

            if (relScaleX != 1 || relScaleY != 1)
            {
                // adjust window size constraints as well
                MinWidth *= relScaleX;
                MaxWidth *= relScaleX;
                MinHeight *= relScaleY;
                MaxHeight *= relScaleY;

                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// Refreshes the current monitor DPI settings and update the window size and layout scale accordingly.
        /// </summary>
        protected void RefreshMonitorDpi()
        {
            if (!isPerMonitorDpiAware)
            {
                return;
            }

            // get the current DPI of the monitor of the window
            var monitor = NativeMethods.MonitorFromWindow(source.Handle, NativeMethods.MONITOR_DEFAULTTONEAREST);

            uint xDpi = 96;
            uint yDpi = 96;
            if (NativeMethods.GetDpiForMonitor(monitor, (int)MonitorDpiType.EffectiveDpi, ref xDpi, ref yDpi) != NativeMethods.S_OK)
            {
                xDpi = 96;
                yDpi = 96;
            }

            // vector contains the change of the old to new DPI
            var dpiVector = dpiInfo.UpdateMonitorDpi(xDpi, yDpi);

            // update Width and Height based on the current DPI of the monitor
            UpdateWindowSize(Width * dpiVector.X, Height * dpiVector.Y);

            // update graphics and text based on the current DPI of the monitor
            UpdateLayoutTransform();
        }

        /// <summary>
        /// Raises the <see cref="E:DpiChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDpiChanged(EventArgs e)
        {
            var handler = DpiChanged;
            handler?.Invoke(this, e);
        }
    }
}
