namespace LightDE.Settings
{
    public partial class Config
    {
        public static Config Current;

        private string _dir;
        private string _prefix;
        private string _suffix;

        public Config(string dir, string prefix, string suffix)
        {
            _dir = dir;
            _prefix = prefix;
            _suffix = suffix;
            Load();
        }

        public void Reload()
        {
            Load();
        }

        private void LoadSingle<T>(ref T o, int i)
        {
            string path = _dir + "\\" + _prefix + i + _suffix;
            if (System.IO.File.Exists(path))
            {
                try
                {
                    o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(path));
                }
                catch
                {
                    // do nothing, default values will be used
                }
            }
        }

        private void SaveSingle(object o, int i)
        {
            string path = _dir + "\\" + _prefix + i + _suffix;
            try
            {
                System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(o));
            }
            catch
            {
                // TODO make error
            }
        }

        private static System.Windows.Controls.TabItem TI(string name, System.Windows.Controls.StackPanel m)
        {
            System.Windows.Controls.TabItem t = new System.Windows.Controls.TabItem();
            t.Name = name;
            t.Header = name;
            t.Content = m;
            return t;
        }
    }
}
