﻿using MinecraftService.Shared.JsonModels.Minecraft;

namespace MinecraftService.Shared.PackParser {
    public class MinecraftPackContainer {
        public PackManifestJsonModel JsonManifest;
        public string PackContentLocation;
        public string ManifestType;
        public string FolderName;
        public byte[] IconBytes;

        public override string ToString() {
            return JsonManifest != null ? $"{JsonManifest.header.name} ({JsonManifest.modules[0].type})" : "WorldPack";
        }

        public string GetFixedManifestType() {
            return ManifestType == "data" ?
                "behavior" : ManifestType == "resources" ?
                "resource" : ManifestType;
        }
    }
}
