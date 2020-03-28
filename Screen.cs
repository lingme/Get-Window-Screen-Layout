using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Gundam.Spike.ScreenInfo
{
    public class Screen
    {
        private readonly IntPtr hmonitor;
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);
        private const int MONITORINFOF_PRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        private static bool multiMonitorSupport;

        public Rect Bounds { get; private set; }
        public string DeviceName { get; private set; }
        public bool Primary { get; private set; }

        static Screen()
        {
            multiMonitorSupport = NativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS) != 0;
        }

        private Screen(IntPtr monitor)
            : this(monitor, IntPtr.Zero)
        {
        }

        private Screen(IntPtr monitor, IntPtr hdc)
        {
            if (!multiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR)
            {
                Bounds = GetVirtualScreen();
                Primary = true;
                DeviceName = "DISPLAY";
            }
            else
            {
                var info = new NativeMethods.MONITORINFOEX();

                NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);

                Bounds = new Rect(
                    info.rcMonitor.left, info.rcMonitor.top,
                    info.rcMonitor.right - info.rcMonitor.left,
                    info.rcMonitor.bottom - info.rcMonitor.top);

                Primary = ((info.dwFlags & MONITORINFOF_PRIMARY) != 0);

                DeviceName = new string(info.szDevice).TrimEnd((char)0);
            }
            hmonitor = monitor;
        }

        private Rect GetVirtualScreen()
        {
            var size = new Size(NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN), NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN));
            return new Rect(0, 0, size.Width, size.Height);
        }

        public static IEnumerable<Screen> AllScreens
        {
            get
            {
                if (multiMonitorSupport)
                {
                    var closure = new MonitorEnumCallback();
                    var proc = new NativeMethods.MonitorEnumProc(closure.Callback);
                    NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, proc, IntPtr.Zero);
                    if (closure.Screens.Count > 0)
                    {
                        return closure.Screens.Cast<Screen>();
                    }
                }
                return new[] { new Screen((IntPtr)PRIMARY_MONITOR) };
            }
        }

        public static Screen PrimaryScreen
        {
            get
            {
                if (multiMonitorSupport)
                {
                    return AllScreens.FirstOrDefault(t => t.Primary);
                }
                return new Screen((IntPtr)PRIMARY_MONITOR);
            }
        }

        public Rect WorkingArea
        {
            get
            {
                if (!multiMonitorSupport || hmonitor == (IntPtr)PRIMARY_MONITOR)
                {
                    return GetWorkingArea();
                }
                var info = new NativeMethods.MONITORINFOEX();
                NativeMethods.GetMonitorInfo(new HandleRef(null, hmonitor), info);
                return new Rect(
                    info.rcWork.left, info.rcWork.top,
                    info.rcWork.right - info.rcWork.left,
                    info.rcWork.bottom - info.rcWork.top);
            }
        }

        private Rect GetWorkingArea()
        {
            NativeMethods.RECT rc = new NativeMethods.RECT();
            NativeMethods.SystemParametersInfo(NativeMethods.SPI_GETWORKAREA, 0, ref rc, 0);
            return new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }

        public static Screen FromHandle(IntPtr hwnd)
        {
            if (multiMonitorSupport)
            {
                return new Screen(NativeMethods.MonitorFromWindow(new HandleRef(null, hwnd), 2));
            }
            return new Screen((IntPtr)PRIMARY_MONITOR);
        }

        public static Screen FromPoint(Point point)
        {
            if (multiMonitorSupport)
            {
                var pt = new NativeMethods.POINTSTRUCT((int)point.X, (int)point.Y);
                return new Screen(NativeMethods.MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST));
            }
            return new Screen((IntPtr)PRIMARY_MONITOR);
        }

        public override bool Equals(object obj)
        {
            var monitor = obj as Screen;
            if (monitor != null)
            {
                if (hmonitor == monitor.hmonitor)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)hmonitor;
        }

        private class MonitorEnumCallback
        {
            public ArrayList Screens { get; private set; }

            public MonitorEnumCallback()
            {
                Screens = new ArrayList();
            }

            public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
            {
                Screens.Add(new Screen(monitor, hdc));
                return true;
            }
        }
    }
}
