using System;
using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;

namespace Draven.Structures.Platform.Summoner
{
    [Serializable]
    [SerializedName("com.riotgames.platform.summoner.spellbook.SpellBookPageDTO")]
    public class SpellBookPageDTO
    {
        [SerializedName("slotEntries")]
        public ArrayCollection SlotEntries { get; set; }

        [SerializedName("summonerId")]
        public double SummonerId { get; set; }

        [SerializedName("createDate")]
        public DateTime CreateDate { get; set; }

        [SerializedName("name")]
        public string Name { get; set; }

        [SerializedName("pageId")]
        public int PageId { get; set; }

        [SerializedName("current")]
        public bool Current { get; set; }
    }
}