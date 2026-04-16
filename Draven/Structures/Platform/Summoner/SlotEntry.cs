using System;
using RtmpSharp.IO;

namespace Draven.Structures.Platform.Summoner
{
    [Serializable]
    [SerializedName("com.riotgames.platform.summoner.spellbook.SlotEntry")]
    public class SlotEntry
    {
        [SerializedName("runeId")]
        public int RuneId { get; set; }

        [SerializedName("runeSlotId")]
        public int RuneSlotId { get; set; }
    }
}