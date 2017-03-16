using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMilkman.Ini;
using System.IO;

namespace LightDE.AppManagement
{
    class AppFetcher
    {
        public AppFetcher()
        {
            Directory.CreateDirectory("Apps");
        }
        string[] PathsToSearch = new string[] {Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) };

        public void GenerateFiles()
        {
            try
            {
                Parallel.ForEach<string>(PathsToSearch, x =>
                {
                    try
                    {
                        Parallel.ForEach<string>(System.IO.Directory.GetFiles(x, "*.ink", System.IO.SearchOption.AllDirectories), a =>
                        {
                            try
                            {
                                IniFile file = new IniFile(); // Create new instance of file
                                IniSection section = file.Sections.Add("App"); // Add new section
                                section.Keys.Add("name", System.IO.Path.GetFileNameWithoutExtension(a)); //Add name key
                                section.Keys.Add("desc", ""); //Add desc key
                                section.Keys.Add("path", Path.GetFullPath(a));
                                section.Keys.Add("iconPath", null);
                                file.Save(Directory.GetCurrentDirectory() + "\\Apps\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".ini");
                            }
                            catch { }
                        });
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                });
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
