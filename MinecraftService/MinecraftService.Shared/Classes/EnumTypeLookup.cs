﻿using MinecraftService.Shared.Classes.Configurations;
using MinecraftService.Shared.Classes.Updaters;
using MinecraftService.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using static MinecraftService.Shared.Classes.SharedStringBase;

namespace MinecraftService.Shared.Classes
{
    public class EnumTypeLookup : IEnumTypeLookup {
        private readonly IServerLogger _logger;
        private readonly IServiceConfiguration _service;
        private Dictionary<MinecraftServerArch, IUpdater> _updatersByArch;

        public EnumTypeLookup(IServerLogger logger, IServiceConfiguration service) {
            _logger = logger;
            _service = service;
            _updatersByArch = new Dictionary<MinecraftServerArch, IUpdater> {
                { MinecraftServerArch.Bedrock, new BedrockUpdater(_logger, _service) },
                { MinecraftServerArch.LiteLoader, new LiteUpdater(_logger, _service) },
                { MinecraftServerArch.Java, new JavaUpdater(_logger, _service) },
            };
        }

        public IUpdater GetUpdaterByArch(MinecraftServerArch arch) => _updatersByArch[arch];

        public IUpdater GetUpdaterByArch(string archName) => _updatersByArch[GetArchFromString(archName)];

        public Dictionary<MinecraftServerArch, IUpdater> GetAllUpdaters() => _updatersByArch;

        public IServerConfiguration PrepareNewServerByArch(string archName, IProcessInfo processInfo, IServerLogger logger, IServiceConfiguration service) => PrepareNewServerByArch(GetArchFromString(archName), processInfo, logger, service);

        public IServerConfiguration PrepareNewServerByArch(MinecraftServerArch archType, IProcessInfo processInfo, IServerLogger logger, IServiceConfiguration service) {
            return archType switch {
                MinecraftServerArch.Bedrock => new BedrockConfiguration(processInfo, logger, service),
                MinecraftServerArch.LiteLoader => new LiteLoaderConfiguration(processInfo, logger, service),
                MinecraftServerArch.Java => new JavaConfiguration(processInfo, logger, service),
                _ => null,
            };
        }
    }
}
