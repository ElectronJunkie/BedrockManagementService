﻿// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using MinecraftService.Service.Core.Interfaces;
global using MinecraftService.Service.Management;
global using MinecraftService.Service.Networking;
global using MinecraftService.Shared.Classes;
global using MinecraftService.Shared.Interfaces;
global using MinecraftService.Shared.Utilities;
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;
global using Topshelf;
using MinecraftService.Service.Core;
using MinecraftService.Service.Networking.Interfaces;
using MinecraftService.Shared.Classes.Server;
using MinecraftService.Shared.Classes.Server.Updaters;
using MinecraftService.Shared.Classes.Service;
using MinecraftService.Shared.Classes.Service.Configuration;
using MinecraftService.Shared.Classes.Service.Core;

namespace MinecraftService.Service
{
    public class Program {
        public static bool IsExiting = false;
        private static readonly string _declaredType = "Service";
        private static bool _isDebugEnabled = false;
        private static bool _shouldStartService = true;
        public static void Main(string[] args) {
            if (args.Length > 0) {
                _isDebugEnabled = args[0].ToLower() == "--debug";
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    ProcessInfo processInfo = new ProcessInfo(_declaredType, Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), Process.GetCurrentProcess().Id, _isDebugEnabled, _shouldStartService);
                    SharedStringBase.SetWorkingDirectory(processInfo);
                    services.AddHostedService<Core.WindowsService>()
                        .AddSingleton(processInfo)
                        .AddTransient<NetworkStrategyLookup>()
                        .AddTransient<FileUtilities>()
                        .AddSingleton<MmsLogger, MmsLogger>()
                        .AddSingleton<ServiceConfigurator>()
                        .AddSingleton<UpdaterContainer>()
                        .AddSingleton<MmsService>()
                        .AddSingleton<ITCPObject, TCPListener>()
                        .AddSingleton<UserConfigManager, UserConfigManager>();
                });
    }
}
