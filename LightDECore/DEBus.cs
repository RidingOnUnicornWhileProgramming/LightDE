using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZetaIpc.Runtime.Server;

namespace LightDE.Core
{
    class DEBus
    {
        internal DEBus()
        {
            var s = new IpcServer();
            s.Start(12345); // Passing no port selects a free port automatically.

            Console.WriteLine("Started server on port {0}.", s.Port);

            s.ReceivedRequest += (sender, args) =>
            {
                if (args.Request == "Volume")
                {
                    args.Response = Program._audio.Volume.ToString();
                }
                if (args.Request == "Next")
                {
                    KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_NEXTTRACK);
                    args.Response = "Next Track";

                }
                if (args.Request == "Prev")
                {
                    KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_PREVIOUSTRACK);
                    args.Response = "Previous Track";
                }
                if (args.Request == "PlayPause")
                {
                    KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_PLAY_PAUSE);
                    args.Response = "Play/Pause Track";
                }
                if (args.Request.StartsWith("GetConfig"))
                {
                    args.Response = Program._config.GetConfigAsString();
                }
                if (args.Request.StartsWith("SetVar"))
                {
                    string[] l = args.Request.Split(new char[] { '|' });
                    Program._config.SetVar(l[1], l[2]);
                    args.Response = "Variable set";
                }
                if (args.Request.StartsWith("GetVar"))
                {
                    string[] l = args.Request.Split(new char[] { '|' });
                    args.Response = Program._config.GetVar(l[1]);
                }
                if (args.Request.StartsWith("AddToMenu"))
                {
                    string[] l = args.Request.Split(new char[] { '|' });
                    Program._appFetcher.AddToMenu(l[1]);
                    args.Response = "item " + l[1] + " added to menu";
                }
                if (args.Request == "ConfigInitialized")
                {
                    args.Response = "OK";
                    Console.WriteLine("Connected with config window...");
                }
                if (args.Request.StartsWith("RemoveFromMenu"))
                {
                    string[] l = args.Request.Split(new char[] { '|' });
                    Program._appFetcher.RemoveFromMenu(l[1]);
                    args.Response = "item " + l[1] + " removed to menu";
                }
                args.Handled = true;
            };
        }
    }
}
