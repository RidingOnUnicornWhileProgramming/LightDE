using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace LightDE.Config
{
    public class ConfigManager
    {
        Dictionary<string, object> config = new Dictionary<string, object>();
        public ConfigManager()
        {
            try
            {
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\Config\\config.json"));
                config = json;
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString());
            }
            GetFile();
        }
        public void SetVar(string sender, string varname, object tosave)
        {
            var configname = sender + "_" + varname;
            if (!config.ContainsKey(configname))
            {
                config.Add(configname, tosave);
            }
            else
            {
                Console.WriteLine(tosave);
                config[configname] = tosave;
                Serialize();
            }
        }
        public Dictionary<string, object> GetConfig()
        {
            return config;
        }
        public Dictionary<string, object> GetConfig(string key)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> o in config)
            {
                if (o.Key.StartsWith(key))
                {
                    dict.Add(o.Key, o.Value);
                }
            }
            return dict;
        }
        public List<string> GetComponents()
        {
            List<string> unique = new List<string>();
            foreach (KeyValuePair<string, object> o in config)
            {
                if (!unique.Contains(o.Key))
                {
                    unique.Add(o.Key.Split(new char[] { '_' } )[0]);
                }

            }
            return unique;
        }
        public object GetVar(string sender, string varname)
        {
            var configname = sender + "_" + varname;

            if (config.ContainsKey(configname))
            {
                return config[configname];
            }
            else return null;
        }
        public void Serialize()
        {
            var json = JsonConvert.SerializeObject(config);
            try
            {

                    File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\Config\\config.json", json);
            }
            catch
            {

            }
        }
        public void GetFile()
        {
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\Config\\config.json"));
            config = json;
        }
    }
    public interface ISaveable
    {
        string name { get; }
    }
}
