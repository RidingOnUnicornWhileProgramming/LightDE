/*   
    WindowManager - Class For managing Windows for DE
    Copyright(C) 2017  Paweł Zmarzły (Piotr Mikstacki only started, all code belongs to Paweł Zmarzły)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<http://www.gnu.org/licenses
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using TsudaKageyu;
using System.Windows;
using System.Windows.Interop;
using System.Threading;

namespace LightDE.WindowManagement
{
    public class GUIItem
    {
        public delegate void DestroyFunc();
        public DestroyFunc Destroy;
    }

    public class WindowManager
    {
        System.Timers.Timer checkwnd = new System.Timers.Timer(500);
        public List<string> IgnoredTitles = new List<string>();
        public List<long> OpenedWindows = new List<long>();
        List<GUIItem> OpenedWindowsGUIItems = new List<GUIItem>();
        List<long> UpdatedOpenedWindows = new List<long>();

        public delegate GUIItem GUIItemFactory(WinHandle window);
        GUIItemFactory fact;

        bool busy = false;
        ~WindowManager()
        {
            checkwnd.Stop();
        }
        public WindowManager(GUIItemFactory GUIItemFact)
        {
            fact = GUIItemFact;
            checkwnd.Elapsed += Checkwnd_Elapsed;
            checkwnd.Start();
            CheckWindows();
        }

        private void Checkwnd_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!busy) CheckWindows();
        }

        private void CheckWindows()
        {
                busy = true;
                UpdatedOpenedWindows.Clear();
                WINAPI.EnumWindows(CountCallback, 0);
                var WindowsStillOpen = OpenedWindows.Intersect(UpdatedOpenedWindows);
                var WindowsToClose = OpenedWindows.Except(WindowsStillOpen).ToList();
                var WindowsToOpen = UpdatedOpenedWindows.Except(WindowsStillOpen).ToList();

                    foreach (long l in WindowsToClose)
                    {
                        int index = OpenedWindows.FindIndex(x =>  x == l );
                            if (index != -1)
                            {
                                    OpenedWindowsGUIItems[index].Destroy();
                                    OpenedWindowsGUIItems.RemoveAt(index);
                                    OpenedWindows.RemoveAt(index);
                            }
                     }
                foreach (long l in WindowsToOpen)
                {
                    OpenedWindows.Add(l);
                    OpenedWindowsGUIItems.Add(fact(new WinHandle(l)));
                }
                busy = false;
        }

        private bool CountCallback(IntPtr hwnd, int lParam)
        {
            if (IsTaskBarWindow(hwnd)) UpdatedOpenedWindows.Add(hwnd.ToInt64());
            return true; //continue enumeration
        }
        private bool IsTaskBarWindow(IntPtr hwnd)
        {
            // TODO make sure it won't pass itself
            if (WINAPI.GetWindowTextLength(hwnd) > 0)
            {
                if ((WINAPI.GetWindowLongA(hwnd, WINAPI.GWL_STYLE)) == WINAPI.TARGETWINDOW || WINAPI.IsWindowVisible(hwnd))
                {
                    bool bNoOwner = (WINAPI.GetWindow(hwnd, WINAPI.GW_OWNER) == IntPtr.Zero);
                    int lExStyle = WINAPI.GetWindowLong(hwnd, WINAPI.GWL_EXSTYLE);
                    if (!bNoOwner) return false;
                    if ((((lExStyle & WINAPI.WS_EX_TOOLWINDOW) == 0)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /*        List<IntPtr> GetRootWindowsOfProcess(int pid)
                {
                    List<IntPtr> rootWindows = GetChildWindows(IntPtr.Zero);
                    List<IntPtr> dsProcRootWindows = new List<IntPtr>();
                    foreach (IntPtr hWnd in rootWindows)
                    {
                        uint lpdwProcessId;
                        WINAPI.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                        if (lpdwProcessId == pid)
                            dsProcRootWindows.Add(hWnd);
                    }
                    return dsProcRootWindows;
                }

                public List<IntPtr> GetChildWindows(IntPtr parent)
                {
                    List<IntPtr> result = new List<IntPtr>();
                    GCHandle listHandle = GCHandle.Alloc(result);
                    try
                    {
                        WINAPI.Win32Callback childProc = new WINAPI.Win32Callback(EnumWindow);
                        WINAPI.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
                    }
                    finally
                    {
                        if (listHandle.IsAllocated)
                            listHandle.Free();
                    }
                    return result;
                }
        */
    }
    public class WinHandle
    {
        static IconExtractor xx = new IconExtractor(@"C:\Windows\System32\imageres.dll");
        static Icon DEF_ICON = xx.GetIcon(11);

        private string _title;
        public string Title
        {
            get
            {
                if (_title != null) return _title;
                _title = GetTitle();
                return _title;
            }
        }
        public readonly IntPtr Ptr;

        System.Timers.Timer chTit = new System.Timers.Timer(500);

        public WinHandle(long ptr)
        {
            Ptr = new IntPtr(ptr);
            chTit.Elapsed += ChTit_Elapsed;
            chTit.Start();
        }

        private void ChTit_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckTitle();
        }

        ~WinHandle()
        {
            chTit.Stop();
            // GC.Collect();
            // GC.WaitForPendingFinalizers();
        }

        private Icon _windowIcon;
        public Icon WindowIcon
        {
            get
            {
                if (_windowIcon != null) return _windowIcon;
                Icon i = WINAPI.CodeUtopia.GetAppIcon(Ptr);
                _windowIcon = i != null ? i : DEF_ICON;
                return _windowIcon;
            }
        }
        private string GetTitle()
        {
            var sb = new StringBuilder(200);
            WINAPI.GetWindowText(Ptr, sb, sb.Capacity);
            return sb.ToString();
        }
        private void CheckTitle()
        {
            string newtitle = GetTitle();
            if (!_title.Equals(newtitle))
            {
                _title = newtitle;
                InvokeTitleChanged(new EventArgs());
            }
        }
        public void HideWindow()
        {
            WINAPI.MoveWindow(Ptr, 0, -50, 0, 0, false);
        }
        public void InvokeTitleChanged(EventArgs e)
        {
            EventHandler handler = TitleChanged;
            if (handler != null) handler(this, e);
        }
        public void MaximizeMinimize()
        {
            if (!WINAPI.IsIconic(Ptr))
            {
                WINAPI.CloseWindow(Ptr);
            }
            else
            {
                WINAPI.ShowWindow(Ptr, 4);
                WINAPI.SetForegroundWindow(Ptr);
            }
        }
        public event EventHandler TitleChanged;
    }
}
