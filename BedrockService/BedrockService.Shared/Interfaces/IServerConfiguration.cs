﻿using BedrockService.Shared.Classes;
using BedrockService.Shared.JsonModels.LiteLoaderJsonModels;
using BedrockService.Shared.SerializeModels;
using System.Collections.Generic;

namespace BedrockService.Shared.Interfaces {
    public interface IServerConfiguration : IBedrockConfiguration {
        int GetRunningPid();
        void SetRunningPid(int runningPid);
        string GetServerName();
        string GetConfigFileName();
        void AddStartCommand(string command);
        bool DeleteStartCommand(string command);
        List<StartCmdEntry> GetStartCommands();
        void SetStartCommands(List<StartCmdEntry> newEntries);
        List<IPlayer> GetPlayerList();
        void SetPlayerList(List<IPlayer> playerList);
        IPlayer GetOrCreatePlayer(string xuid, string username = null);
        IServerConfiguration GetServerInfo();
        LiteLoaderConfigNodeModel GetLiteLoaderConfig();
        void SetLiteLoaderConfig(LiteLoaderConfigNodeModel config);
        void SetServerVersion(string newVersion);
        string GetServerVersion();
        string GetSelectedVersion();
        void SetStatus(ServerStatusModel status);
        ServerStatusModel GetStatus();
        void SetBackupTotals(int totalBackups, int totalSize);
        Property GetSettingsProp(SharedStringBase.ServerPropertyKeys name);
        void SetSettingsProp(SharedStringBase.ServerPropertyKeys key, string value);
        void SetSettingsProp(string key, string value);
        List<Property> GetSettingsList();
        void SetAllSettings(List<Property> settingsList);
        bool ValidateVersion(string version, bool skipNullCheck = false);
        void SetLiteLoaderStatus(bool statusToSet);
        bool GetLiteLoaderStatus();
    }
}
