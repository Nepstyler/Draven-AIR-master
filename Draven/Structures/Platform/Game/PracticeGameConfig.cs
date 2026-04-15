namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;

    [Serializable]
    [SerializedName("com.riotgames.platform.game.PracticeGameConfig")]
    public class PracticeGameConfig
    {
        [SerializedName("gameName")]
        public string GameName { get; set; }

        [SerializedName("gamePassword")]
        public string GamePassword { get; set; }

        [SerializedName("gameMode")]
        public string GameMode { get; set; }

        [SerializedName("maxNumPlayers")]
        public int MaxNumPlayers { get; set; }

        // Aici am corectat din ASObject în AsObject
        [SerializedName("gameMap")]
        public AsObject GameMap { get; set; }

        [SerializedName("gameTypeConfig")]
        public int GameTypeConfig { get; set; }
    }
}