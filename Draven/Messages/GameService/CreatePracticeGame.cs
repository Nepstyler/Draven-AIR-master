using System;
using Draven.ServerModels;
using Draven.Structures.Platform.Game;
using RtmpSharp.Messaging;
using RtmpSharp.IO.AMF3;

namespace Draven.Messages.GameService
{
    public class CreatePracticeGame : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            // 1. Extragem datele trimise de clientul de LOL
            PracticeGameConfig config = null;

            // Corectură: e.Body este un obiect (de obicei un array de argumente trimise de client)
            object[] bodyArgs = e.Body as object[];
            if (bodyArgs != null && bodyArgs.Length > 0)
            {
                config = bodyArgs[0] as PracticeGameConfig;
            }

            if (config == null) return e;

            // mapId=1 inseamna Summoners Rift vechi, 11 e noul SR. Setam default 11.
            int mapId = 11;
            if (config.GameMap != null && config.GameMap.ContainsKey("mapId"))
            {
                mapId = Convert.ToInt32(config.GameMap["mapId"]);
            }

            Console.WriteLine($"[LOG] Creating Practice Game '{config.GameName}' on Map {mapId}");

            // 2. Inserăm Lobby-ul în Baza de Date
            int lobbyId = DatabaseManager.DatabaseManager.CreateLobby(
                Convert.ToInt32(client._accId),
                config.GameName,
                mapId,
                config.GameMode
            );

            // Inserăm Proprietarul în Echipa 1 (Blue)
            DatabaseManager.DatabaseManager.AddParticipantToLobby(lobbyId, Convert.ToInt32(client._accId), 1);

            // 3. Creăm Profilul tău pentru ecranul de lobby
            PlayerParticipant owner = new PlayerParticipant
            {
                AccountId = Convert.ToDouble(client._accId),
                SummonerId = Convert.ToDouble(client._accId),
                SummonerName = client._session.Summary.Username,
                SummonerInternalName = client._session.Summary.Username.ToLower(),
                ProfileIconId = 1,
                TeamOwner = true
            };

            // 4. Generăm starea Camerei (GameDTO)
            GameDTO gameDto = new GameDTO
            {
                Id = lobbyId,
                Name = config.GameName,
                RoomName = "draven_room_" + lobbyId,
                RoomPassword = config.GamePassword,
                GameState = "TEAM_SELECT",
                GameMode = config.GameMode,
                GameType = "PRACTICE_GAME",
                MapId = mapId,
                MaxNumPlayers = config.MaxNumPlayers,
                SpectatorsAllowed = "NONE",
                OwnerSummary = owner,
                TeamOne = new ArrayCollection { owner }, // Te bagă automat în echipa din stânga
                TeamTwo = new ArrayCollection(),
                Observers = new ArrayCollection(),
                BannedChampions = new ArrayCollection(),
                PlayerChampionSelections = new ArrayCollection()
            };

            // 5. Trimitem datele înapoi la Client
            e.ReturnRequired = true;
            e.Data = gameDto;

            return e;
        }
    }
}