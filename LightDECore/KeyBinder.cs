using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;

namespace LightDE.Core
{
    public class KeyBinder
    {
        public List<KeyBinding> KeyBinds = new List<KeyBinding>();
        public KeyBinder()
        {
            HookManager.KeyUp += HookManager_KeyUp;
            Console.WriteLine("KeyBinder init done. now you can user control keys on your keyboard");
        }
        bool mute;
        double vol;
        private void HookManager_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bool known = false;
            switch (e.KeyCode.ToString())
            {
                case "VolumeUp":
                    try
                    {

                        Program._audio.Volume = Program._audio.Volume + 0.6;
                    }
                    catch { }
                    break;
                case "VolumeDown":
                    try
                    {
                        Program._audio.Volume = Program._audio.Volume - 0.6;
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
                            vol = Program._audio.Volume;
                            Program._audio.Volume = 0;
                        }
                        else
                        {
                            Program._audio.Volume = vol;
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
