/*
 * this class comes from https://hianz.wordpress.com/2013/09/03/new-windows-tray-notification-manager-is-here/
 * Kudos to this guy he made notifyIcons possible
 * It's really  awesome that there exists somewhere someone with such Possibilities
 * PS. I could do this on my own, managed but I'm little lazy and this is only FreeTime Project so...
*/
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
namespace LightDE
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
    }

    public class NOTIFYITEMICON : INotifyPropertyChanged
    {
        public NOTIFYITEM original { get; set; }
        public string pszExeName { get; set; }
        public string pszTip { get; set; }
        public IntPtr hIcon { get; set; }
        public string hWnd { get; set; }
        public uint dwPreference;
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
        [DllImport("TrayNotificationManagement.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void PerformActionWithCallBack(IntPtr pCallBack);

        [DllImport("TrayNotificationManagement.DLL")]
        public static extern void Begin();

        [DllImport("TrayNotificationManagement.DLL")]
        public static extern void EnableAutoTrayFunction(bool enabled);

        [DllImport("TrayNotificationManagement.DLL")]
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

                temp.image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(temp.hIcon, new Int32Rect(0, 0, 16, 16), BitmapSizeOptions.FromEmptyOptions());

       
                temp.PropertyChanged += new PropertyChangedEventHandler(TempOnPropertyChanged);


                programs.Add(temp);
            }
        }

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
    }
}
