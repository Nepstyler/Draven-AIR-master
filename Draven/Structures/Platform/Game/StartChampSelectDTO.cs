namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;
    using RtmpSharp.IO.AMF3;

    [Serializable]
    [SerializedName("com.riotgames.platform.game.StartChampSelectDTO")]
    public class StartChampSelectDTO
    {
        [SerializedName("invalidPlayers")]
        public ArrayCollection InvalidPlayers { get; set; }
    }
}