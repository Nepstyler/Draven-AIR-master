using System;
using Draven.ServerModels;
using Draven.Structures.Platform.Game;
using RtmpSharp.Messaging;
using RtmpSharp.IO.AMF3;

namespace Draven.Messages.GameService
{
    public class StartChampionSelection : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            object[] bodyArgs = e.Body as object[];
            int lobbyId = 0;
            if (bodyArgs != null && bodyArgs.Length > 0)
            {
                lobbyId = Convert.ToInt32(bodyArgs[0]);
            }

            Console.WriteLine($"[LOG] Starting Champion Select for Lobby {lobbyId}");

            PlayerChampionSelectionDTO selection = new PlayerChampionSelectionDTO
            {
                SummonerInternalName = client._session.Summary.Username.ToLower(),
                ChampionId = 0,
                SelectedSkinIndex = 0,
                Spell1Id = 7,
                Spell2Id = 4
            };

            PlayerParticipant owner = new PlayerParticipant
            {
                AccountId = Convert.ToDouble(client._accId),
                SummonerId = Convert.ToDouble(client._accId),
                SummonerName = client._session.Summary.Username,
                SummonerInternalName = client._session.Summary.Username.ToLower(),
                ProfileIconId = 1,
                TeamOwner = true,
                PickTurn = 1,
                PickMode = 1,
                ClientInSynch = false,
                BotDifficulty = "NONE",
                OriginalPlatformId = "NA",
                PartnerId = "",
                OriginalAccountNumber = Convert.ToDouble(client._accId)
            };

            GameDTO updatedGameDto = new GameDTO
            {
                Id = lobbyId,
                Name = "Draven Practice Game",
                RoomName = "draven_room_" + lobbyId,
                RoomPassword = "",
                GameState = "CHAMP_SELECT",
                GameStateString = "CHAMP_SELECT",
                GameMode = "CLASSIC",
                GameType = "PRACTICE_GAME",
                GameTypeConfigId = 1,
                MapId = 11,
                MaxNumPlayers = 10,
                QueueTypeName = "NONE",
                QueuePosition = 0,
                OptimisticLock = 1.0,
                PickTurn = 1,
                ExpiryTime = 2678203.0,
                JoinTimerDuration = 0,
                SpectatorsAllowed = "NONE",
                TerminatedCondition = "NOT_TERMINATED",
                TerminatedConditionString = "NOT_TERMINATED",

                OwnerSummary = owner,
                TeamOne = new ArrayCollection { owner },
                TeamTwo = new ArrayCollection(),
                Observers = new ArrayCollection(),
                BannedChampions = new ArrayCollection(),
                BanOrder = new ArrayCollection(),
                GameMutators = new ArrayCollection(),
                PracticeGameRewardsDisabledReasons = new ArrayCollection { "INSUFFICIENT_PLAYERS" },
                PlayerChampionSelections = new ArrayCollection { selection },

                FeaturedGameInfo = new FeaturedGameInfo
                {
                    DataVersion = 1,
                    ChampionVoteInfoList = new ArrayCollection()
                }
            };

            client._rtmpClient.InvokeDestReceive("gn-" + client._accId, "gn-" + client._accId, "messagingDestination", updatedGameDto);

            StartChampSelectDTO response = new StartChampSelectDTO
            {
                InvalidPlayers = new ArrayCollection()
            };

            e.ReturnRequired = true;
            e.Data = response;

            return e;
        }
    }
}