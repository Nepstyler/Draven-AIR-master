using System;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace Draven.Structures.Platform.Summoner
{
    [Serializable]
    [SerializedName("com.riotgames.platform.summoner.runes.SummonerRuneInventory")]
    public class SummonerRuneInventory
    {
        [SerializedName("summonerRunes")]
        public ArrayCollection SummonerRunes { get; set; }

        [SerializedName("summonerId")]
        public double SummonerId { get; set; }

        [SerializedName("dateString")]
        public string DateString { get; set; }
    }
}