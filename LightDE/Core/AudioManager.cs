using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDE.Core
{
    class AudioManager
    {
        internal CoreAudioDevice _device;
        internal AudioManager()
        {
            try
            {
                _device = new CoreAudioController().DefaultPlaybackDevice;
                Console.WriteLine("Audio device initialized. Device name is: " + _device.FullName);
            }
            catch
            {
                Console.WriteLine("cannot find audio device");
            }

        }
        public double Volume
        {
            get
            {
                return _device.Volume;
            }
            set
            {
                try
                {
                    _device.Volume = value;
                }
                catch {
                    Console.WriteLine("cannot find audio device");
                }
            }
        }
    }
}
