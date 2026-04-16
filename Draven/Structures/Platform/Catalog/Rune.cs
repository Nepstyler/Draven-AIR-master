using System;
using RtmpSharp.IO;

namespace Draven.Structures.Platform.Catalog
{
    [Serializable]
    [SerializedName("com.riotgames.platform.catalog.runes.Rune")]
    public class Rune
    {
        [SerializedName("imagePath")] public string ImagePath { get; set; }
        [SerializedName("toolTip")] public string ToolTip { get; set; }
        [SerializedName("tier")] public int Tier { get; set; }
        [SerializedName("itemId")] public int ItemId { get; set; }
        [SerializedName("runeType")] public RuneType RuneType { get; set; }
        [SerializedName("name")] public string Name { get; set; }
        [SerializedName("description")] public string Description { get; set; }
    }
}