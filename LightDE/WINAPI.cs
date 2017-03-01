using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace LightDE.WindowManagement
{
    /// <summary>
    /// Added just to tidy up Project. All the WINAPI calls goes here...
    ///
    /// </summary>
    static class WINAPI
    {
        [DllImport("user32.dll")]
        public extern static uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
        public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam); // callback for enum windows

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);
        public const int GWL_STYLE = (-16);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        public extern static void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public extern static bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public extern static bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public extern static int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        public extern static bool EndTask(IntPtr hWnd, bool fShutDown, bool fForce);

        public static IntPtr HWND_TOPMOST = (IntPtr)(-1);
        public static int SWP_NOSIZE = 0x1;

        [DllImport("user32.dll")]
        public extern static int SetWindowPos(IntPtr hwnd, IntPtr
        hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }
        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        [DllImport("user32.dll")]
        public extern static int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        extern public static ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public extern static bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static IntPtr GetWindow(IntPtr hWnd, int wFlag);

        [DllImport("user32.dll")]
        public extern static int GetWindowLong(IntPtr hWnd, int nIndex);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        extern public static int GetWindowTextLength(IntPtr hWnd);
        public static readonly ulong WS_VISIBLE = 0x10000000L;
        public static readonly ulong WS_BORDER = 0x00800000L;
        public static readonly ulong TARGETWINDOW = WS_VISIBLE;
        public static readonly int WS_EX_TOOLWINDOW = 0x80;
        public static readonly int WS_EX_APPWINDOW = 0x40000;
        public static readonly int GW_OWNER = 4;
        public static readonly int GWL_EXSTYLE = -20;

        public static class CodeUtopia
        {
            // https://codeutopia.net/blog/2007/12/18/find-an-applications-icon-with-winapi/

            public const int GCL_HICONSM = -34;
            public const int GCL_HICON = -14;

            public const int ICON_SMALL = 0;
            public const int ICON_BIG = 1;
            public const int ICON_SMALL2 = 2;

            public const int WM_GETICON = 0x7F;

            public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
            {
                if (IntPtr.Size > 4)
                    return GetClassLongPtr64(hWnd, nIndex);
                else
                    return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
            }

            public static Icon GetAppIcon(IntPtr hwnd)
            {
                IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

                if (iconHandle == IntPtr.Zero)
                    return null;

                Icon icn = Icon.FromHandle(iconHandle);

                return icn;
            }

            [DllImport("user32.dll", EntryPoint = "GetClassLong")]
            public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
            public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        }
    }
    enum ShowWindowCommands
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
                      /// <summary>
                      /// Activates the window and displays it as a maximized window.
                      /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }
}