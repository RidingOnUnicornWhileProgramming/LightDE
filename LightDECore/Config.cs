using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightDE.Core
{
    class Config
    {
        internal Dictionary<string, string> vars = new Dictionary<string, string>();
        internal Config()
        {
            Console.WriteLine("Initializing config...");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Config");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Config\\config.json"))
            {
                vars = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Config\\config.json"));
            }
            Console.WriteLine("Config init done.");
        }
        public void SetVar(string key, string var)
        {
            if (vars.ContainsKey(key))
            {
                vars[key] = var;
            }
            else
            {
                vars.Add(key, var); 
            }
            Serialize();
        }
        public string GetVar(string key)
        {
            if (vars.ContainsKey(key))
            {
                return vars[key];
            }
            else
            {
                return "not found";
            }
        }
        public string GetConfigAsString()
        {
            return JsonConvert.SerializeObject(vars);
        }
        public void Serialize()
        {
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Config\\config.json", JsonConvert.SerializeObject(vars));
        }
    }
}
