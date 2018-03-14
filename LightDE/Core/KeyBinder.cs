using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;
using System.Threading;
using LightDE.UI;

namespace LightDE.Core
{
    public class KeyBinder
    {
        public static KeyBinder _current;
        public List<KeyBinding> KeyBinds = new List<KeyBinding>();
        public KeyBinder()
        {
            _current = this;
            HookManager.KeyUp += HookManager_KeyUp;
            Console.WriteLine("KeyBinder init done. now you can user control keys on your keyboard");
        }
        bool mute;
        double vol;
        private void HookManager_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            new Thread(new ThreadStart(() => {
            bool known = false;
            switch (e.KeyCode.ToString())
            {
                case "VolumeUp":
                    try
                    {

                        MainWindow._current.defaultPlaybackDevice.Volume = MainWindow._current.defaultPlaybackDevice.Volume + 0.6;
                    }
                    catch { }
                    break;
                case "VolumeDown":
                    try
                    {
                        MainWindow._current.defaultPlaybackDevice.Volume = MainWindow._current.defaultPlaybackDevice.Volume - 0.6;
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
                            vol = MainWindow._current.defaultPlaybackDevice.Volume;
                            MainWindow._current.defaultPlaybackDevice.Volume = 0;
                        }
                        else
                        {
                            MainWindow._current.defaultPlaybackDevice.Volume = vol;
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
            })).Start();
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
