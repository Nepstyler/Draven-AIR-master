using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Draven.ServerModels;
using Draven.Structures.Platform.Game;
using Draven.Helpers;
using Newtonsoft.Json;
using RtmpSharp.Messaging;
using RtmpSharp.IO.AMF3;
using System.Threading.Tasks;

namespace Draven.Messages.GameService
{
    public class ChampionSelectCompleted : IMessage
    {
        // Path to the game server files
        private string GameInfoPath = @"D:\Sandbox\TestGrounds\GameServerConsole\bin\Debug\net8.0\Settings\GameInfo.json";
        private string GameServerPath = @"D:\Sandbox\TestGrounds\GameServerConsole\bin\Debug\net8.0\GameServerConsole.exe";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            // Convert long skin ID to short index (e.g., 157001 -> 1)
            int shortSkinIndex = LobbyState.SkinIndex % 1000;

            Console.WriteLine("[LOG] Finalizing selection for " + client._session.Summary.Username);

            // 1. Generate GameInfo.json using strict C# 7.3 syntax
            var gameInfo = new
            {
                players = new List<object> {
                    new {
                        playerId = 1,
                        blowfishKey = "17BLOhi6KZsTtldTsizvHg==",
                        rank = "DIAMOND",
                        name = client._session.Summary.Username,
                        champion = ConstantsTranslator.GetChampionName(LobbyState.ChampionId),
                        team = "BLUE",
                        skin = shortSkinIndex,
                        summoner1 = ConstantsTranslator.GetSpellName(LobbyState.Spell1Id),
                        summoner2 = ConstantsTranslator.GetSpellName(LobbyState.Spell2Id),
                        ribbon = 0,
                        icon = 10,
                        runes = new Dictionary<string, int> {
                            { "1", 5251 }, { "2", 5251 }, { "3", 5251 }, { "4", 5251 }, { "5", 5251 },
                            { "6", 5245 }, { "7", 5245 }, { "8", 5245 }, { "9", 5245 }, { "10", 5317 },
                            { "11", 5317 }, { "12", 5317 }, { "13", 5317 }, { "14", 5317 }, { "15", 5317 },
                            { "16", 5317 }, { "17", 5317 }, { "18", 5311 }, { "19", 5289 }, { "20", 5289 },
                            { "21", 5289 }, { "22", 5289 }, { "23", 5289 }, { "24", 5289 }, { "25", 5289 },
                            { "26", 5289 }, { "27", 5289 }, { "28", 5335 }, { "29", 5335 }, { "30", 5335 }
                        },
                        talents = new Dictionary<string, int> {
                            { "4111", 1 }, { "4112", 3 }, { "4114", 1 }, { "4122", 3 }, { "4124", 1 },
                            { "4132", 1 }, { "4134", 3 }, { "4142", 3 }, { "4151", 1 }, { "4152", 3 },
                            { "4162", 1 }, { "4211", 2 }, { "4213", 2 }, { "4221", 1 }, { "4222", 3 }, { "4232", 1 }
                        }
                    }
                },
                game = new
                {
                    map = 1,
                    gameMode = "CLASSIC",
                    dataPackage = "LeagueSandbox-Scripts"
                },
                gameInfo = new
                {
                    MANACOSTS_ENABLED = false,
                    COOLDOWNS_ENABLED = true,
                    CHEATS_ENABLED = true,
                    MINION_SPAWNS_ENABLED = false,
                    CONTENT_PATH = "../../../../../Content",
                    IS_DAMAGE_TEXT_GLOBAL = false
                },
                forcedStart = 120
            };

            try
            {
                // Serialize and write GameInfo.json
                string jsonString = JsonConvert.SerializeObject(gameInfo, Formatting.Indented);
                File.WriteAllText(GameInfoPath, jsonString);
                Console.WriteLine("[LOG] GameInfo.json generated.");

                // 2. DIRECT LAUNCH: Execute GameServerConsole.exe immediately
                ProcessStartInfo si = new ProcessStartInfo
                {
                    FileName = GameServerPath,
                    WorkingDirectory = Path.GetDirectoryName(GameServerPath),
                    UseShellExecute = true
                };
                Process.Start(si);
                Console.WriteLine("[SUCCESS] Emulator console launched!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Launch failed: " + ex.Message);
            }

            // 3. Sync UI state to freeze the champion select screen
            PlayerParticipant owner = new PlayerParticipant
            {
                AccountId = Convert.ToDouble(client._accId),
                SummonerId = Convert.ToDouble(client._accId),
                SummonerName = client._session.Summary.Username,
                PickMode = 2, // 2 = Locked In
                ClientInSynch = true
            };

            GameDTO syncDto = new GameDTO
            {
                Id = 1,
                GameState = "STARTING",
                GameStateString = "STARTING",
                OwnerSummary = owner,
                TeamOne = new ArrayCollection { owner },
                FeaturedGameInfo = new FeaturedGameInfo { DataVersion = 1, ChampionVoteInfoList = new ArrayCollection() }
            };

            // Signal to the Air Client that the game is starting
            client._rtmpClient.InvokeDestReceive("gn-" + client._accId, "gn-" + client._accId, "messagingDestination", syncDto);

            e.ReturnRequired = true;
            e.Data = null;
            return e;
        }
    }
}