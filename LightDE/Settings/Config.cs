using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;

namespace LightDE.Settings
{
    public partial class Config
    {
        public ConfigV1 V1 = new ConfigV1();

        private void Load()
        {
            if (Directory.Exists(_dir))
            {
                LoadSingle(ref V1, 1);
            }
        }

        public void Save()
        {
            SaveSingle(V1, 1);
        }

        public List<TabItem> SettingsWnd()
        {
            List<TabItem> tabitems = new List<TabItem>();

            StackPanel m; TabItem t; Setting s;

            m = new StackPanel();
            t = TI("General", m);

            s = new Setting("First run", ref V1.General_FirstRun);
            m.Children.Add(s);

            tabitems.Add(t);


            m = new StackPanel();
            t = TI("Desktop", m);

            s = new Setting("Wallpaper path", ref V1.DesktopD_WallpaperPath);
            m.Children.Add(s);

            tabitems.Add(t);

            return tabitems;
        }
    }

    public class ConfigV1
    {
        public List<string> Apps_AppNames = new List<string>();

        public List<string> DesktopD_RSS = new List<string>() { "http://www.techradar.com/rss" };
        public string DesktopD_WallpaperPath = "";

        public bool General_FirstRun = true;
    }
}
