﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MinecraftService.Client.Management;
using MinecraftService.Shared.Classes.Service.Configuration;
using MinecraftService.Shared.Interfaces;

namespace MinecraftService.Client.Forms {
    public partial class ClientConfigForm : Form {
        private readonly List<ClientSideServiceConfiguration> _clientConfigs;
        private readonly ConfigManager _configManager;
        public ClientConfigForm(ConfigManager configManager) {
            InitializeComponent();
            _configManager = configManager;
            _clientConfigs = _configManager.HostConnectList;
            scrollLockCheckbox.Checked = _configManager.DefaultScrollLock;
            displayTimestampCheckbox.Checked = _configManager.DisplayTimestamps;
            debugNetworkCheckbox.Checked = _configManager.DebugNetworkOutput;
            if (!string.IsNullOrEmpty(_configManager.NBTStudioPath)) {
                nbtPathLabel.Text = $"NBT Studio path: {_configManager.NBTStudioPath}";
            }
            foreach (ClientSideServiceConfiguration config in _clientConfigs) {
                serverGridView.Rows.Add(new string[3] { config.GetHostName(), config.GetAddress(), config.GetPort() });
            }
        }

        public void SimulateTests() {
            nbtButton.PerformClick();
        }

        private void nbtButton_Click(object sender, EventArgs e) {
            using (OpenFileDialog fileDialog = new()) {
                fileDialog.Filter = "EXE Files|*.exe";
                fileDialog.FileName = "NbtStudio.exe";
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (fileDialog.ShowDialog() == DialogResult.OK) {
                    _configManager.NBTStudioPath = fileDialog.FileName;
                    nbtPathLabel.Text = $"NBT Studio path: {_configManager.NBTStudioPath}";
                }
            }
        }

        private void saveBtn_Click(object sender, EventArgs e) {
            List<ClientSideServiceConfiguration> newConfigs = new();
            foreach (DataGridViewRow row in serverGridView.Rows) {
                if (!string.IsNullOrEmpty((string)row.Cells[0].Value)) {
                    newConfigs.Add(new ClientSideServiceConfiguration((string)row.Cells[0].Value, (string)row.Cells[1].Value, (string)row.Cells[2].Value));
                }
            }
            _configManager.HostConnectList = newConfigs;
            _configManager.DefaultScrollLock = scrollLockCheckbox.Checked;
            _configManager.DisplayTimestamps = displayTimestampCheckbox.Checked;
            _configManager.DebugNetworkOutput = debugNetworkCheckbox.Checked;
            _configManager.SaveConfigFile();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
