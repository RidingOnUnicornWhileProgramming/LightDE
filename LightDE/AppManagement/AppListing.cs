using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace LightDE
{
    public class AppsListing
    {
        public List<xApp> GetItems()
        {
            List<xApp> appslist = new List<xApp>();
            List<xApp> menuApp = new List<xApp>();
            try
            {
                Parallel.ForEach<string>(Directory.GetDirectories(@"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs"), d =>
                {
                    Parallel.ForEach<string>(Directory.GetFiles(d), f =>
                    {
                        if (!f.EndsWith(".ini"))
                        {
                            appslist.Add(new xApp(ExtractIcon.GetName(f), System.Drawing.Icon.ExtractAssociatedIcon(f).ToBitmap(), f));
                        }
                    });
                });
                try
                {
                    Parallel.ForEach<string>(Directory.GetDirectories(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"), dd =>
                    {
                        try
                        {
                            Parallel.ForEach<string>(Directory.GetFiles(dd), ff =>
                            {
                                try
                                {
                                    if (!ff.EndsWith(".ini"))
                                    {
                                        if (!appslist.Any(x => x.name == (ExtractIcon.GetName(ff))))
                                        {
                                            menuApp.Add(new xApp(ExtractIcon.GetName(ff), System.Drawing.Icon.ExtractAssociatedIcon(ff).ToBitmap(), ff));
                                        }
                                    }
                                }
                                catch { }
                            });
                        }
                        catch { }
                    });
                }
                catch { }
                appslist.AddRange(menuApp);
                menuApp = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return appslist;
        }

    }
    public class ExtractIcon
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFileInfo(
          string pszPath,
          int dwFileAttributes,
          out SHFILEINFO psfi,
          uint cbfileInfo,
          SHGFI uFlags);

        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
        private const int MAX_PATH = 260;
        /// <summary>Maximal Length of unmanaged Typename</summary>
        private const int MAX_TYPE = 80;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
            public string szTypeName;
        };

        private ExtractIcon()
        {
        }

        [Flags]
        enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon.  If the strPath is invalid or there is no idonc the default icon is returned
        /// </summary>
        /// <param name="strPath">full path to the file</param>
        /// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
        /// <returns></returns>
        public static System.Drawing.Icon GetIcon(string strPath, bool bSmall)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            return System.Drawing.Icon.FromHandle(info.hIcon);
        }
        public static string GetName(string strPath)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            flags = SHGFI.DisplayName;
            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            return info.szDisplayName;
        }
    }
    public class xApp
    {
        public string name;
        public Bitmap icon;
        public string Path;
        public bool seen;
        public xApp(string name, Bitmap icon, string Path)
        {
            this.name = name;
            this.icon = icon;
            this.Path = Path;
        }
    }

}
