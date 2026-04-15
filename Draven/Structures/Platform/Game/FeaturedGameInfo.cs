namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;
    using RtmpSharp.IO.AMF3;

    [Serializable]
    [SerializedName("com.riotgames.platform.game.FeaturedGameInfo")]
    public class FeaturedGameInfo
    {
        [SerializedName("championVoteInfoList")] public ArrayCollection ChampionVoteInfoList { get; set; }
        [SerializedName("dataVersion")] public int DataVersion { get; set; }
    }
}