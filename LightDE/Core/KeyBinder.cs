using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDE.Core
{
    public class KeyBinder
    {
        public List<KeyBinding> KeyBinds = new List<KeyBinding>();
        public KeyBinder()
        {
            HookManager.KeyPress += HookManager_KeyPress;
            HookManager.KeyUp += HookManager_KeyUp;
            HookManager.MouseMove += HookManager_MouseMove;
        }
        bool mute;
        double vol;
        private void HookManager_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bool known = false;
            Console.WriteLine(e.KeyCode);
            switch (e.KeyCode.ToString())
            {
                case "VolumeUp":
                    try
                    {

                        MainWindow.instance.Volume.Value = MainWindow.instance.Volume.Value + 0.6;
                    }
                    catch { }
                    break;
                case "VolumeDown":
                    try
                    {
                        MainWindow.instance.Volume.Value = MainWindow.instance.Volume.Value - 0.6;
                    }
                    catch { }
                    break;
                case "MediaPlayPause":
                    try
                    {
                        KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_PLAY_PAUSE);
                    }
                    catch { }
                    break;
                case "VolumeMute":
                    try
                    {
                        mute = !mute;
                        if (mute)
                        {
                            vol = MainWindow.instance.Volume.Value;
                            MainWindow.instance.Volume.Value = 0;
                        }
                        else
                        {
                            MainWindow.instance.Volume.Value = vol;
                        }
                    }
                    catch { }
                    break;
                default:
                    if(!KeyBinds.Contains(KeyBinds.Find(x => x.key == e.KeyCode.ToString())))
                    KeyBinds.Add(new KeyBinding() { key = e.KeyCode.ToString() });
                    break;
            }
            KeyBinding key = KeyBinds.Find(x => x.key == e.KeyCode.ToString());
            if (key != null)
            {
                if (key.Code != KeyManager.AppComandCode.NULL)
                {
                    KeyManager.AppCommand(key.Code);
                }
            }
        }

        private void HookManager_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void HookManager_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
        }
    }
    [System.Serializable]
    public class KeyBinding
    {
        public string key
        {
            get;
            set;
        }
        public KeyManager.AppComandCode Code = KeyManager.AppComandCode.NULL;
    }
}
