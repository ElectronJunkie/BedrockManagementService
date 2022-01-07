﻿using BedrockService.Client.Forms;
using BedrockService.Client.Networking;
using BedrockService.Shared.Classes;
using BedrockService.Shared.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BedrockService.Client.Management {
    public sealed class FormManager {
        private static readonly IProcessInfo processInfo = new ServiceProcessInfo("Client", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Process.GetCurrentProcess().Id, false, true);
        private static readonly IServiceConfiguration _configuration;
        private static readonly IBedrockLogger _logger;
        private static MainWindow main;
        private static TCPClient client;

        static FormManager() {
            _configuration = new ServiceConfigurator(processInfo);
            _configuration.InitializeDefaults();
            _configuration.SetProp(new Property("LogApplicationOutput", "true") { Value = "true" });
            _logger = new BedrockLogger(processInfo, _configuration);
        }

        public static MainWindow MainWindow {
            get {
                if (main == null || main.IsDisposed) {
                    main = new MainWindow(processInfo, _logger);
                }
                return main;
            }
        }

        public static TCPClient TCPClient {
            get {
                if (client == null) {
                    client = new TCPClient(_logger);
                }
                return client;
            }
        }
    }
}
