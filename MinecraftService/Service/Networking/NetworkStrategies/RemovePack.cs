﻿using MinecraftService.Service.Networking.Interfaces;
using MinecraftService.Shared.Classes.Networking;
using MinecraftService.Shared.Classes.Service;
using MinecraftService.Shared.Classes.Service.Configuration;
using MinecraftService.Shared.Classes.Service.Core;
using MinecraftService.Shared.PackParser;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using static MinecraftService.Shared.Classes.Service.Core.SharedStringBase;

namespace MinecraftService.Service.Networking.NetworkStrategies
{
    public class RemovePack(ServiceConfigurator serviceConfiguration, MmsLogger logger) : IMessageParser {

        public Message ParseMessage(Message message) {
            string stringData = Encoding.UTF8.GetString(message.Data, 5, message.Data.Length - 5);
            string pathToWorldFolder = $@"{serviceConfiguration.GetServerInfoByIndex(message.ServerIndex).GetSettingsProp(ServerPropertyKeys.ServerPath)}\worlds\{serviceConfiguration.GetServerInfoByIndex(message.ServerIndex).GetProp(MmsDependServerPropKeys.LevelName)}";
            MinecraftKnownPacksClass knownPacks = new($@"{serviceConfiguration.GetServerInfoByIndex(message.ServerIndex).GetSettingsProp(ServerPropertyKeys.ServerPath)}\valid_known_packs.json", pathToWorldFolder);
            List<MinecraftPackContainer>? container = JsonConvert.DeserializeObject<List<MinecraftPackContainer>>(stringData, SharedStringBase.GlobalJsonSerialierSettings);
            foreach (MinecraftPackContainer content in container) {
                knownPacks.RemovePackFromServer(serviceConfiguration.GetServerInfoByIndex(message.ServerIndex), content);
                logger.AppendLine($@"{content.JsonManifest.header.name} removed from server.");
            }
            return Message.EmptyUICallback;
        }
    }
}

