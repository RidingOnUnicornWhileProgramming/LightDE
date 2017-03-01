using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
namespace LightDE.WindowManagement
{
   
    [StructLayout(LayoutKind.Sequential)]
    public struct GUID
    {
        public int a;
        public short b;
        public short c;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] d;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYITEM
    {
        //[MarshalAs(UnmanagedType.LPWStr)]
        public string pszExeName { get; set; }
        //[MarshalAs(UnmanagedType.LPWStr)]
        public string pszTip { get; set; }
        public IntPtr hIcon { get; set; }
        public IntPtr hWnd { get; set; }
        public uint dwPreference { get; set; }
        public uint uID { get; set; }
        public GUID GuidItem { get; set; }
        public uint uCallbackMessage { get; set; }

    }

    public class NOTIFYITEMICON : INotifyPropertyChanged
    {
        public NOTIFYITEM original { get; set; }
        public string pszExeName { get; set; }
        public string pszTip { get; set; }
        public IntPtr hIcon { get; set; }
        public string hWnd { get; set; }
        public uint dwPreference;
        public uint CallbackMessage
        {
            get;
            set;
        }

        public uint DwPreference
        {
            get { return dwPreference; }
            set
            {
                if (dwPreference != value)
                {
                    dwPreference = value;
                    OnPropertyChanged("DwPreference");
                }
            }
        }
        public uint uID { get; set; }
        public GUID GuidItem { get; set; }
        public IntPtr pointerToOriginal { get; set; }
        public BitmapSource image { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    

    class NotifyIconManager : INotifyPropertyChanged
    {
        ObservableCollection<NOTIFYITEMICON> programs;

        public ObservableCollection<NOTIFYITEMICON> Programs
        {
            get { return programs; }
            set
            {
                if (value != programs)
                {
                    programs = value;
                    OnPropertyChanged("Programs");
                }
            }
        }

        // Declare a delegate that represents a managed callback function.
        public delegate void CallBack(IntPtr notifyitem);
        public static int count = 0;

        // Declare the API that will be called in managed code.
        // The API will in turn call the managed callback before it returns.
        [DllImport("TrayNotificationManagementWin8.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void PerformActionWithCallBack(IntPtr pCallBack);

        [DllImport("TrayNotificationManagementWin8.DLL")]
        public static extern void Begin();

        [DllImport("TrayNotificationManagementWin8.DLL")]
        public static extern void EnableAutoTrayFunction(bool enabled);

        [DllImport("TrayNotificationManagementWin8.DLL")]
        public static extern void SetPreferenceFunction(IntPtr item);

        public static Icon bild;
        private static CallBack callback_delegate;
        private static IntPtr intptr_delegate;

        //public static List<NOTIFYITEMICON> programs = new List<NOTIFYITEMICON>();

        private void DoCallBack()
        {
            // Create an instance of a CallBack delegate for CallBackFunction().
             callback_delegate = new CallBack(CallBackFunction);
            // Convert callback_delegate into a function pointer that can be
            // used in unmanaged code.
             intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
            // Call the API passing along the function pointer.
            PerformActionWithCallBack(intptr_delegate);
        }
        public static void PostWindowsMessage(IntPtr hWnd, uint callback, uint uid, uint messageId)
        {
            PostMessage(hWnd, callback, uid, messageId);
        }
        // This managed function will be called from unmanaged code.
        private void CallBackFunction(IntPtr notifyitem)
        {
            if (notifyitem.ToInt32() != 0)
            {
                count++;
                NOTIFYITEM item = (NOTIFYITEM)Marshal.PtrToStructure(notifyitem, typeof(NOTIFYITEM));
                NOTIFYITEMICON temp = new NOTIFYITEMICON();

                temp.original = item;
                temp.pszExeName = item.pszExeName;
                temp.pszTip = item.pszTip;
                temp.hIcon = item.hIcon;
                if (item.hWnd.ToInt32() == 0)
                    temp.hWnd = "No Window present";
                else
                    temp.hWnd = item.hWnd.ToString();

                temp.dwPreference = item.dwPreference;
                temp.uID = item.uID;
                temp.GuidItem = item.GuidItem;
                temp.pointerToOriginal = notifyitem;
                bild = Icon.FromHandle(temp.hIcon);
                temp.CallbackMessage = item.uCallbackMessage;
                temp.image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(temp.hIcon, new Int32Rect(0, 0, 16, 16), BitmapSizeOptions.FromEmptyOptions());

       
                temp.PropertyChanged += new PropertyChangedEventHandler(TempOnPropertyChanged);


                programs.Add(temp);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint callback, uint wParam, uint lParam);

        private void TempOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NOTIFYITEMICON icon = (NOTIFYITEMICON) sender;
            NOTIFYITEM newItem = new NOTIFYITEM();
            
            newItem.GuidItem = icon.original.GuidItem;
            newItem.uID = icon.original.uID;
            newItem.dwPreference = 2;
            newItem.hWnd = icon.original.hWnd;
            newItem.hIcon = icon.original.hIcon;
            newItem.pszTip = icon.original.pszTip;
            newItem.pszExeName = icon.original.pszExeName;

            IntPtr ptr = icon.pointerToOriginal;
            Marshal.StructureToPtr(newItem, ptr, false);
            SetPreferenceFunction(ptr);

        }

        public  void DoStuff()
        {
            programs = new ObservableCollection<NOTIFYITEMICON>();
            DoCallBack();
            Begin();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
        /// </summary>
        /// <remarks>The EnumChildWindows function is more reliable than calling GetWindow in a loop. An application that
        /// calls GetWindow to perform this task risks being caught in an infinite loop or referencing a handle to a window
        /// that has been destroyed.</remarks>
        /// <param name="hWnd">A handle to a window. The window handle retrieved is relative to this window, based on the
        /// value of the uCmd parameter.</param>
        /// <param name="uCmd">The relationship between the specified window and the window whose handle is to be
        /// retrieved.</param>
        /// <returns>
        /// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship
        /// to the specified window, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        public enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized 
            /// or maximized, the system restores it to its original size and 
            /// position. An application should specify this flag when displaying 
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position. 
            /// This value is similar to "ShowNormal", except the window is not 
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size 
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next 
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is 
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This 
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is 
            /// minimized or maximized, the system restores it to its original size 
            /// and position. An application should specify this flag when restoring 
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
            /// that owns the window is hung. This flag should only be used when 
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }
        public enum GetWindowType : uint
        {
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is highest in the Z order.
            /// <para/>
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDFIRST = 0,
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDLAST = 1,
            /// <summary>
            /// The retrieved handle identifies the window below the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDNEXT = 2,
            /// <summary>
            /// The retrieved handle identifies the window above the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDPREV = 3,
            /// <summary>
            /// The retrieved handle identifies the specified window's owner window, if any.
            /// </summary>
            GW_OWNER = 4,
            /// <summary>
            /// The retrieved handle identifies the child window at the top of the Z order,
            /// if the specified window is a parent window; otherwise, the retrieved handle is NULL.
            /// The function examines only child windows of the specified window. It does not examine descendant windows.
            /// </summary>
            GW_CHILD = 5,
            /// <summary>
            /// The retrieved handle identifies the enabled popup window owned by the specified window (the
            /// search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled
            /// popup windows, the retrieved handle is that of the specified window.
            /// </summary>
            GW_ENABLEDPOPUP = 6
        }

    }
}
