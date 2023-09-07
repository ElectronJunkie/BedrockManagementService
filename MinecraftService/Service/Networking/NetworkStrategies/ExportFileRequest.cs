﻿
using MinecraftService.Service.Networking.Interfaces;
using MinecraftService.Shared.SerializeModels;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Text;
using static MinecraftService.Shared.Classes.SharedStringBase;

namespace MinecraftService.Service.Networking.NetworkStrategies {
    public class ExportFileRequest : IMessageParser {
        private readonly IServiceConfiguration _configuration;
        private readonly IConfigurator _configurator;
        public ExportFileRequest(IConfigurator configurator, IServiceConfiguration configuration) {
            _configuration = configuration;
            _configurator = configurator;
        }

        public (byte[] data, byte srvIndex, NetworkMessageTypes type) ParseMessage(byte[] data, byte serverIndex) {
            string jsonString = Encoding.UTF8.GetString(data, 5, data.Length - 5);
            ExportImportFileModel exportFileInfo = JsonConvert.DeserializeObject<ExportImportFileModel>(jsonString);
            using MemoryStream ms = new();
            using ZipArchive packageFile = new(ms, ZipArchiveMode.Create);
            byte[]? exportData = null;
            if (serverIndex != 255 && exportFileInfo != null) {
                if (exportFileInfo.FileType == FileTypeFlags.Backup) {
                    IServerConfiguration server = _configuration.GetServerInfoByIndex(serverIndex);
                    string backupPath = $"{server.GetSettingsProp(ServerPropertyKeys.BackupPath)}\\{server.GetServerName()}\\{exportFileInfo.Filename}";
                    exportFileInfo.Data = File.ReadAllBytes(backupPath);
                }
                if (exportFileInfo.FileType == FileTypeFlags.ServerPackage) {
                    IServerConfiguration server = _configuration.GetServerInfoByIndex(serverIndex);

                    PrepareServerFiles(serverIndex, exportFileInfo, server, packageFile);
                    packageFile.Dispose();
                    exportFileInfo.Data = ms.ToArray();
                }
                exportData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(exportFileInfo));
            }
            if (exportFileInfo.FileType == FileTypeFlags.ServicePackage) {
                packageFile.CreateEntryFromFile(GetServiceFilePath(MmsFileNameKeys.ServiceConfig), GetServiceFilePath(MmsFileNameKeys.ServiceConfig));
            }
            return (exportData, 0, NetworkMessageTypes.ExportFile);
        }


        private void PrepareServerFiles(byte serverIndex, ExportImportFileModel exportFileInfo, IServerConfiguration server, ZipArchive packageFile) {
            if (exportFileInfo.PackageFlags >= PackageFlags.ConfigFile) {
                packageFile.CreateEntryFromFile(GetServiceFilePath(MmsFileNameKeys.ServerConfig_Name, server.GetServerName()), GetServiceFileName(MmsFileNameKeys.ServerConfig_Name, server.GetServerName()));
            }
            if (exportFileInfo.PackageFlags >= PackageFlags.LastBackup) {
                BackupInfoModel lastBackup = _configurator.EnumerateBackupsForServer(serverIndex).Result.FirstOrDefault();
                if (lastBackup != null) {
                    packageFile.CreateEntryFromFile($"{server.GetSettingsProp(ServerPropertyKeys.BackupPath)}\\{server.GetServerName()}\\{lastBackup.Filename}", lastBackup.Filename);
                }
            }
            if (exportFileInfo.PackageFlags >= PackageFlags.WorldPacks) {
                FileUtilities.CreatePackBackupFiles(server.GetSettingsProp(ServerPropertyKeys.ServerPath).ToString(), server.GetProp(MmsDependServerPropKeys.LevelName).ToString(), packageFile);
            }
            if (exportFileInfo.PackageFlags >= PackageFlags.PlayerDatabase) {
                if (File.Exists(GetServiceFilePath(MmsFileNameKeys.ServerPlayerTelem_Name, server.GetServerName()))) {
                    packageFile.CreateEntryFromFile(GetServiceFilePath(MmsFileNameKeys.ServerPlayerTelem_Name, server.GetServerName()), GetServiceFileName(MmsFileNameKeys.ServerPlayerTelem_Name, server.GetServerName()));
                }
                if (File.Exists(GetServiceFilePath(MmsFileNameKeys.ServerPlayerRegistry_Name, server.GetServerName()))) {
                    packageFile.CreateEntryFromFile(GetServiceFilePath(MmsFileNameKeys.ServerPlayerRegistry_Name, server.GetServerName()), GetServiceFileName(MmsFileNameKeys.ServerPlayerRegistry_Name, server.GetServerName()));
                }
            }
        }
    }
}