using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMilkman.Ini;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace LightDE.AppManagement
{
    class AppFetcher
    {
        public AppFetcher()
        {
            Directory.CreateDirectory("Apps");
            Directory.CreateDirectory("Apps\\Icons");

        }
        string[] PathsToSearch = new string[] { @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs", @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs" };

        public void GenerateFiles()
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
                                IniFile file = new IniFile(); // Create new instance of file
                                IniSection section = file.Sections.Add("App"); // Add new section
                                section.Keys.Add("name", System.IO.Path.GetFileNameWithoutExtension(a)); //Add name key
                                section.Keys.Add("desc", ""); //Add desc key
                                using (MemoryStream memory = new MemoryStream())
                                {
                                    using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\Apps\Icons\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                    {
                                        System.Drawing.Icon.ExtractAssociatedIcon(a).ToBitmap().Save(memory, ImageFormat.Jpeg);
                                        byte[] bytes = memory.ToArray();
                                        fs.Write(bytes, 0, bytes.Length);
                                    }
                                }                            
                                section.Keys.Add("path", Path.GetFullPath(a));
                                section.Keys.Add("iconPath", Directory.GetCurrentDirectory() + "\\Apps\\Icons\\" + System.IO.Path.GetFileNameWithoutExtension(a)+".jpg");
                                file.Save(Directory.GetCurrentDirectory() + "\\Apps\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".ini");
                            }
                            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                        });
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                });
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
