﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MinecraftService.Shared.Classes.Networking;

namespace MinecraftService.Client.Networking {
    internal interface INetworkMessage {
        Task<bool> ProcessMessage(Message message);
    }
}
