using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaIpc.Runtime.Client;

namespace LightDE.UI
{
    class UIClient
    {
        IpcClient _client = new IpcClient();
        internal UIClient()
        {
            
            _client.Initialize(12345);

            Console.WriteLine("Started client.");

            var rep = _client.Send("Initialized");
        }
        public string Request(string req)
        {
            try
            {
                return _client.Send(req);
            }
            catch {
                Console.WriteLine("Connection Error! \n make sure that Core is running.");
                return null;
            }
        }
    }
}
