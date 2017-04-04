using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaIpc.Runtime.Client;

namespace LightDEDock
{
    class DockClient
    {
        internal DockClient()
        {
            var c = new IpcClient();
            c.Initialize(12345);

            Console.WriteLine("Started client.");

            var rep = c.Send("Hello");
            Console.WriteLine("Received: " + rep);
        }
    }
}
