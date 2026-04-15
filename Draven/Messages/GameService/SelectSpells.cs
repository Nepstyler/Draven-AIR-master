using System;
using Draven.ServerModels;
using Draven.Structures.Platform.Game;
using RtmpSharp.Messaging;
using RtmpSharp.IO.AMF3;

namespace Draven.Messages.GameService
{
    public class SelectSpells : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;
            object[] bodyArgs = e.Body as object[];

            if (bodyArgs != null && bodyArgs.Length >= 2)
            {
                // Save the new spells to server memory
                LobbyState.Spell1Id = Convert.ToInt32(bodyArgs[0]);
                LobbyState.Spell2Id = Convert.ToInt32(bodyArgs[1]);
            }

            Console.WriteLine($"[LOG] {client._session.Summary.Username} changed spells to: {LobbyState.Spell1Id} and {LobbyState.Spell2Id}");

            // Build selection using existing memory so champion isn't lost
            PlayerChampionSelectionDTO selection = new PlayerChampionSelectionDTO
            {
                SummonerInternalName = client._session.Summary.Username.ToLower(),
                ChampionId = LobbyState.ChampionId,
                SelectedSkinIndex = LobbyState.SkinIndex,
                Spell1Id = LobbyState.Spell1Id,
                Spell2Id = LobbyState.Spell2Id
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
                ClientInSynch = false
            };

            GameDTO updatedGameDto = new GameDTO
            {
                Id = 1,
                Name = "Draven Practice Game",
                RoomName = "draven_room_1",
                RoomPassword = "",
                GameState = "CHAMP_SELECT",
                GameStateString = "CHAMP_SELECT",
                GameMode = "CLASSIC",
                GameType = "PRACTICE_GAME",
                GameTypeConfigId = 1,
                MapId = 11,
                MaxNumPlayers = 10,
                QueueTypeName = "NONE",
                OptimisticLock = 1.0,
                PickTurn = 1,
                OwnerSummary = owner,
                TeamOne = new ArrayCollection { owner },
                TeamTwo = new ArrayCollection(),
                Observers = new ArrayCollection(),
                BannedChampions = new ArrayCollection(),
                PlayerChampionSelections = new ArrayCollection { selection },
                FeaturedGameInfo = new FeaturedGameInfo { DataVersion = 1, ChampionVoteInfoList = new ArrayCollection() }
            };

            // Broadcast the update so spells visually change
            client._rtmpClient.InvokeDestReceive("gn-" + client._accId, "gn-" + client._accId, "messagingDestination", updatedGameDto);

            e.ReturnRequired = true;
            e.Data = null;
            return e;
        }
    }
}