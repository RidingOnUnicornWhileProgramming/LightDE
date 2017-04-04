using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;

namespace LightDE.Core
{
    class Program
    {
        internal static AudioManager _audio;
        internal static KeyBinder _keyBinder;
        internal static Config _config;
        internal static AppFetcher _appFetcher;
        internal static bool force;
        static void Main(string[] args)
        {
            var options = new CoreArgs();
            Console.WriteLine( "This program is free software;" + Environment.NewLine +" you can redistribute it and / or modify it under the terms of the GNU General Public License as published bythe Free Software Foundation;" +Environment.NewLine +" either version 2 of the License, or (at your option) any later version.This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY;" + Environment.NewLine +" without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.");
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                force = options.forceRefresh;                
            }
            _audio = new AudioManager();
            _keyBinder = new KeyBinder();
            if (options.forceRefresh)
            {
                Console.WriteLine("Force Refreshing");

                _appFetcher = new AppFetcher();
                _appFetcher.GenerateFiles(true);
            }
            else
            {
                Console.WriteLine("fetching changes in apps");

                _appFetcher = new AppFetcher();
                _appFetcher.GenerateFiles(false);
            }
            _config = new Config();
            new DEBus(); 
            Console.ReadKey(true);

        }
    }
    class CoreArgs
    {
        [Option('r', "force-refresh", DefaultValue = false, Required = false, HelpText = "force refreshes apps files.")]
        public bool forceRefresh { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

}
