namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;

    // This class holds the credentials required by the League of Legends client
    // to connect to the dedicated PVP Game Server via UDP.
    [Serializable]
    [SerializedName("com.riotgames.platform.game.PlayerCredentialsDto")]
    public class PlayerCredentialsDto
    {
        // The IP address of the PVP Game Server (from run.bat)
        [SerializedName("serverIp")]
        public string ServerIp { get; set; }

        // The port of the PVP Game Server (5119 from run.bat)
        [SerializedName("serverPort")]
        public int ServerPort { get; set; }

        // The Blowfish encryption key for secure UDP communication
        [SerializedName("observerEncryptionKey")]
        public string ObserverEncryptionKey { get; set; }

        // The ID of the summoner connecting to the game
        [SerializedName("summonerId")]
        public double SummonerId { get; set; }

        // The display name of the summoner
        [SerializedName("summonerName")]
        public string SummonerName { get; set; }

        // The unique ID of the match/lobby
        [SerializedName("gameId")]
        public double GameId { get; set; }

        // The player ID assigned by the server (Usually matches SummonerId in emulators)
        [SerializedName("playerId")]
        public double PlayerId { get; set; }

        [SerializedName("handshakeToken")]
        public string HandshakeToken { get; set; }

        [SerializedName("championId")]
        public int ChampionId { get; set; }

        [SerializedName("observerServerIp")]
        public string ObserverServerIp { get; set; }

        [SerializedName("observerServerPort")]
        public int ObserverServerPort { get; set; }
    }
}