﻿using BedrockService.Service.Server;

namespace BedrockService.Service.Core.Interfaces {
    public interface IBedrockService : ServiceControl {
        Task Initialize();
        void RemoveBedrockServerByIndex(int serverIndex);
        void InitializeNewServer(IServerConfiguration serverConfiguration);
        Task RestartService();
        ServiceStatus GetServiceStatus();
        IBedrockServer GetBedrockServerByIndex(int index);
        IBedrockServer? GetBedrockServerByName(string name);
        List<IBedrockServer> GetAllServers();
    }
}
