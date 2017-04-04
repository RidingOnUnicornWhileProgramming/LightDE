using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaIpc.Runtime.Client;

namespace LightDE.Config
{
    class ConfigClient
    {
        IpcClient _client = new IpcClient();
        internal ConfigClient()
        {

            _client.Initialize(12345);

            Console.WriteLine("Started client.");

            var rep = _client.Send("Initialized");
        }
        public string GetConfig()
        {
            try
            {
                return _client.Send("GetConfig");
            }
            catch
            {
                Console.WriteLine("Connection Error! \n make sure that Core is running.");
                return null;
            }
        }
        public void SetVar(string key, string var)
        {
            try
            {
                _client.Send(string.Format("SetVar|{0}|{1}", key, var));
            }
            catch
            {
                Console.WriteLine("Connection Error! \n make sure that Core is running.");
            }
        }
        public string GetVar(string key)
        {
            try
            {
                return _client.Send(string.Format("GetVar|{0}", key));
            }
            catch
            {
                Console.WriteLine("Connection Error! \n make sure that Core is running.");
                return null;
            }
        }
    }
}
