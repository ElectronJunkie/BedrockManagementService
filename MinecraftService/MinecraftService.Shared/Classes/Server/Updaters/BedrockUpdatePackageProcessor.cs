﻿using MinecraftService.Shared.Classes.Service.Core;
using MinecraftService.Shared.Interfaces;
using MinecraftService.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static MinecraftService.Shared.Classes.Service.Core.SharedStringBase;

namespace MinecraftService.Shared.Classes.Server.Updaters
{
    public class BedrockUpdatePackageProcessor
    {
        private readonly string _packageVersion;
        private readonly string _fileTargetDirectory;
        private readonly string _workingDirectory;
        private readonly MmsLogger _logger;
        private readonly bool _loggingEnabled = true;


        public BedrockUpdatePackageProcessor(MmsLogger logger, string packageVersion, string fileTargetDirectory)
        {
            _packageVersion = packageVersion;
            _fileTargetDirectory = fileTargetDirectory;
            _workingDirectory = GetNewTempDirectory("UpdateProc");
            Directory.CreateDirectory(_workingDirectory);
            _logger = logger;
        }

        public BedrockUpdatePackageProcessor(string packageVersion, string fileTargetDirectory)
        {
            _packageVersion = packageVersion;
            _fileTargetDirectory = fileTargetDirectory;
            _workingDirectory = GetNewTempDirectory("UpdateProc");
            _loggingEnabled = false;
        }

        public bool ExtractCoreFiles()
        {
            try
            {
                FileUtilities.CreateInexistantDirectory(_workingDirectory);
                string zipPath = GetServiceFilePath(MmsFileNameKeys.BdsUpdatePackage_Ver, _packageVersion);
                if (!File.Exists(zipPath))
                {
                    if (_loggingEnabled) _logger.AppendLine("Requested build package was not found.");
                    return false;
                }
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    int fileCount = archive.Entries.Count;
                    for (int i = 0; i < fileCount; i++)
                    {
                        int percentResult = 1;
                        try
                        {
                            percentResult = i % ((int)Math.Round(fileCount / 10.0) * 10 / 6);
                        }
                        catch (DivideByZeroException) { }
                        if (percentResult == 0)
                        {
                            if (_loggingEnabled) _logger.AppendLine($"Extracting server files to build core files, {Math.Round(i / (double)fileCount, 2) * 100}% completed...");
                        }
                        string fixedPath = $@"{_workingDirectory}\{archive.Entries[i].FullName.Replace('/', '\\')}";
                        if (!archive.Entries[i].FullName.EndsWith("/"))
                        {
                            if (File.Exists(fixedPath))
                            {
                                File.Delete(fixedPath);
                            }
                            FileInfo fileInfo = new(fixedPath);
                            if (fileInfo.Extension == ".properties")
                            {
                                archive.Entries[i].ExtractToFile(fixedPath);
                                CreateFiles();
                                break;
                            }
                        }
                    }
                    if (_loggingEnabled)
                    {
                        _logger.AppendLine($"Extraction completed.");
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                if (_loggingEnabled) _logger.AppendLine($"Error extracting core files. {e.Message}");
                return false;
            }
        }

        private void CreateFiles()
        {
            if (_loggingEnabled)
            {
                _logger.AppendLine($"Now building necessary files");
            }
            Directory.CreateDirectory(_fileTargetDirectory);
            string propFile = $@"{_workingDirectory}\{GetServerFileName(ServerFileNameKeys.ServerProps)}";

            List<string> propFileContents = new(File.ReadAllLines(propFile));
            propFileContents = propFileContents
                .Where(x => !x.StartsWith('#'))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
            File.WriteAllLines($@"{_fileTargetDirectory}\{GetServiceFileName(MmsFileNameKeys.BedrockStockProps_Ver, _packageVersion)}", propFileContents);
        }
    }
}
