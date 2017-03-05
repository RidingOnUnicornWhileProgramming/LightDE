using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

using Cairo.WindowsHooksWrapper;
using DE.WindowManagement;
using System.Windows;

namespace LightDE
{
    public class NOTIFYITEMICON
    {
        public NOTIFYICONDATA original { get; set; }
        public IntPtr hWnd { get; set; }
        public IntPtr hIcon { get; set; }
        public BitmapSource image { get; set; }

        public string Title;
        public uint UID;

        //public string pszExeName { get; set; }
        //public string pszTip { get; set; }
        // rest? https://gist.github.com/pzmarzly/c787363ef66cf4bf8193d19515b27f40
    }

    class NotifyIconManager
    {
        private Func<NOTIFYITEMICON, GUIItem> addNewNotification;
        IWindowsHooksWrapper hooksWrapper = new WindowsHooksWrapper();
        private SystrayDelegate trayDelegate;

        public NotifyIconManager(Func<NOTIFYITEMICON, GUIItem> addNewNotification)
        {
            this.addNewNotification = addNewNotification;

            // from https://github.com/cairoshell/cairoshell/blob/36de406a00e9b6a8907ba088033f309c81ba096b/code/Cairo%20Desktop/Cairo%20Desktop/SystemTray.xaml.cs
            
            

            trayDelegate = new SystrayDelegate(SysTrayCallback);
            hooksWrapper.SetSystrayCallback(trayDelegate);
            hooksWrapper.InitializeSystray();
            hooksWrapper.Run();
        }

        private object _lockObject = new object();
        private Dictionary<uint, GUIItem> destructors = new Dictionary<uint, GUIItem>();

        private bool SysTrayCallback(uint message, NOTIFYICONDATA nicData)
        {
            var trayIcon = new NOTIFYITEMICON();

            lock (_lockObject)
            {
                switch ((NIM)message)
                {
                    case NIM.NIM_ADD:
                        // Ensure the icon doesn't already exist.
                        if (destructors.ContainsKey(nicData.uID)) return false;  // TODO test it

                        // Add the icon.
                        trayIcon.Title = nicData.szTip;
                        trayIcon.hIcon = nicData.hIcon;
                        trayIcon.original = nicData;
                        try
                        {
                            trayIcon.image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(nicData.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        }
                        catch
                        {
                            trayIcon.image = null;
                        }
                        trayIcon.hWnd = nicData.hWnd;
                        trayIcon.UID = nicData.uID;

                        destructors.Add(nicData.uID, addNewNotification(trayIcon));
                        break;

                    case NIM.NIM_DELETE:
                        try
                        {
                            if (!destructors.ContainsKey(nicData.uID))
                            {
                                // Nothing to remove.
                                return false;
                            }

                            destructors[nicData.uID].Destroy();
                            destructors.Remove(nicData.uID);
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case NIM.NIM_MODIFY:
                        /* TODO IMPLEMENT
                        try
                        {
                            
                            bool exists = false;
                            if (TrayIcons.Contains(trayIcon))
                            {
                                exists = true;
                                trayIcon = TrayIcons.Single(i => i.HWnd == nicData.hWnd && i.UID == nicData.uID);
                            }

                            trayIcon.Title = nicData.szTip;
                            trayIcon.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(nicData.hIcon, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                            trayIcon.HWnd = nicData.hWnd;
                            trayIcon.UID = nicData.uID;
                            trayIcon.CallbackMessage = nicData.uCallbackMessage;

                            if (!exists)
                            {
                                TrayIcons.Add(trayIcon);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("Unable to modify the icon in the collection. Error: " + ex.ToString());
                        }
                        */
                        break;
                }
            }
            return true;
        }
    }
}
