﻿using BedrockService.Shared.Interfaces;
using Newtonsoft.Json;
using System;

namespace BedrockService.Shared.Classes {
    public class Player : IPlayer {
        [JsonProperty]
        private string Username { get; set; }
        [JsonProperty]
        private string XUID { get; set; }
        [JsonProperty]
        private string PermissionLevel;
        [JsonProperty]
        private long FirstConnectedTime { get; set; }
        [JsonProperty]
        private long LastConnectedTime { get; set; }
        [JsonProperty]
        private long LastDisconnectTime { get; set; }
        [JsonProperty]
        private string ServerDefaultPerm { get; set; }
        [JsonProperty]
        private bool Whitelisted { get; set; }
        [JsonProperty]
        private bool IgnorePlayerLimits { get; set; }

        [JsonConstructor]
        public Player(string xuid, string username, long firstConn, long lastConn, long lastDiscon, bool whtlist, string perm, bool ignoreLimit) {
            Username = username;
            XUID = xuid;
            FirstConnectedTime = firstConn;
            LastConnectedTime = lastConn;
            LastDisconnectTime = lastDiscon;
            Whitelisted = whtlist;
            PermissionLevel = perm;
            IgnorePlayerLimits = ignoreLimit;
        }

        public Player(string serverDefaultPermission) {
            ServerDefaultPerm = serverDefaultPermission;
            PermissionLevel = serverDefaultPermission;
        }

        public IPlayer Initialize(string xuid, string username) {
            XUID = xuid;
            Username = username;
            FirstConnectedTime = DateTime.Now.Ticks;
            LastConnectedTime = DateTime.Now.Ticks;
            LastDisconnectTime = DateTime.Now.Ticks;
            return this;
        }

        public string GetUsername() => Username;

        public string SearchForProperty(string input) {
            input = input.ToLower();
            if (input == "name" || input == "username" || input == "un")
                return Username.ToLower();
            if (input == "xuid" || input == "id")
                return XUID;
            if (input == "perm" || input == "permission" || input == "pl")
                return PermissionLevel.ToLower();
            if (input == "whitelist" || input == "white" || input == "wl")
                return Whitelisted.ToString().ToLower();
            if (input == "ignoreslimit" || input == "il")
                return IgnorePlayerLimits.ToString().ToLower();
            return null;
        }

        public string GetXUID() => XUID;

        public (long First, long Conn, long Disconn) GetTimes() {
            return (FirstConnectedTime, LastConnectedTime, LastDisconnectTime);
        }

        public void UpdateTimes(long conn, long disconn) {
            LastConnectedTime = conn;
            LastDisconnectTime = disconn;
        }

        public bool IsDefaultRegistration() {
            return Whitelisted == false && IgnorePlayerLimits == false && PermissionLevel == ServerDefaultPerm;
        }

        public string ToString(string format) {
            if (format == "Known") {
                return $"{XUID},{Username},{FirstConnectedTime},{LastConnectedTime},{LastDisconnectTime}";
            }
            if (format == "Registered") {
                return $"{XUID},{Username},{PermissionLevel},{Whitelisted},{IgnorePlayerLimits}";
            }
            return null;
        }

        public IPlayer UpdatePlayerFromDbStrings(string[] dbString) {
            if (dbString == null || dbString[0] != XUID) {
                throw new ArgumentException("Input null or Player update attempted with incorrect xuid!");
            }
            Username = dbString[1];
            if (!long.TryParse(dbString[2], out long first) || !long.TryParse(dbString[3], out long conn) || !long.TryParse(dbString[4], out long disconn)) {
                throw new InvalidOperationException("Could not parse player times, check logs!");
            }
            FirstConnectedTime = first;
            LastConnectedTime = conn;
            LastDisconnectTime = disconn;
            return this;
        }

        public IPlayer UpdatePlayerFromRegStrings(string[] regString) {
            if (regString == null || regString[0] != XUID) {
                throw new ArgumentException("Input null or Player update attempted with incorrect xuid!");
            }
            Username = regString[1];
            PermissionLevel = regString[2];
            if (!bool.TryParse(regString[3], out bool whiteList) || !bool.TryParse(regString[3], out bool ignoreLimits)) {
                throw new InvalidOperationException("Could not parse registration bools, check configs!");
            }
            Whitelisted = whiteList;
            IgnorePlayerLimits = ignoreLimits;
            return this;
        }

        public bool IsPlayerWhitelisted() => Whitelisted;

        public bool PlayerIgnoresLimit() => IgnorePlayerLimits;

        public string GetPermissionLevel() => PermissionLevel;

        public override bool Equals(object obj) {
            return obj is Player player &&
                   Username == player.Username &&
                   XUID == player.XUID;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Username, XUID);
        }
    }

}