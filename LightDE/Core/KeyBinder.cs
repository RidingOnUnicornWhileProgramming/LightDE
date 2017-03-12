using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDE.Core
{
    public class KeyBinder
    {
        public KeyboardListener keyListener = new KeyboardListener();
        public List<KeyBinding> KeyBinds
        {
            get { return KeyBinds; }
            set { KeyBinds = value; }
        }
        public KeyBinder()
        {

        }
    }
    [System.Serializable]
    public class KeyBinding
    {
        public ushort Key
        {
            get;
            set;
        }
        public KeyManager.AppComandCode Code
        {
            get;
            set;
        }
    }
}
