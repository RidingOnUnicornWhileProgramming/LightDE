using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMilkman.Ini;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace LightDE.Core
{
    class AppFetcher
    {
        public AppFetcher()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\Icons");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\Icons");
            Console.WriteLine("Preparing launch shortcuts...");
        }
        string[] PathsToSearch = new string[] { @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs", @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs" };
        public void AddToMenu(string name)
        {
            File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\" + name, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\" + name);
            File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\Icons\\" + name, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\Icons\\" + name);
        }
        public void RemoveFromMenu(string name)
        {
            File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\" + name, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\" + name);
            File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\Icons\\" + name, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\Icons\\" + name);
        }
        public void GenerateFiles(bool force)
        {
            try
            {
                try
                {
                    Parallel.ForEach<string>(PathsToSearch, x =>
                    {
                        try
                        {
                            Parallel.ForEach<string>(System.IO.Directory.GetFiles(x, "*.lnk", System.IO.SearchOption.AllDirectories), a =>
                            {
                                try
                                {
                                    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\LightDE" + "\\Apps\\All\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".app") || force)
                                        {
                                        IniFile file = new IniFile(); // Create new instance of file
                                        IniSection section = file.Sections.Add("App"); // Add new section
                                        section.Keys.Add("name", System.IO.Path.GetFileNameWithoutExtension(a)); //Add name key
                                        section.Keys.Add("desc", ""); //Add desc key
                                        using (MemoryStream memory = new MemoryStream())
                                        {
                                            using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + @"\Apps\All\Icons\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                            {
                                                System.Drawing.Icon.ExtractAssociatedIcon(a).ToBitmap().Save(memory, ImageFormat.Jpeg);
                                                byte[] bytes = memory.ToArray();
                                                fs.Write(bytes, 0, bytes.Length);
                                                fs.Dispose();
                                            }
                                            memory.Dispose();
                                        }
                                        section.Keys.Add("path", Path.GetFullPath(a));
                                        section.Keys.Add("iconPath", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\Icons\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".jpg");

                                        file.Save(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE"+ "\\Apps\\All\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".app");

                                    }
                                }
                                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                                
                            });
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                    });
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            finally
            {
                Console.WriteLine("App Fetching Done.");
            }
        }
    }
}
