using System;
using RtmpSharp.IO;

namespace Draven.Structures.Platform.Summoner
{
    [Serializable]
    [SerializedName("com.riotgames.platform.summoner.masterybook.TalentEntry")]
    public class TalentEntry
    {
        [SerializedName("talentId")]
        public int TalentId { get; set; }

        [SerializedName("rank")]
        public int Rank { get; set; }
    }
}