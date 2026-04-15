namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;

    [Serializable]
    [SerializedName("com.riotgames.platform.game.PlayerParticipant")]
    public class PlayerParticipant
    {
        [SerializedName("accountId")] public double AccountId { get; set; }
        [SerializedName("summonerId")] public double SummonerId { get; set; }
        [SerializedName("summonerName")] public string SummonerName { get; set; }
        [SerializedName("summonerInternalName")] public string SummonerInternalName { get; set; }
        [SerializedName("profileIconId")] public int ProfileIconId { get; set; }
        [SerializedName("teamOwner")] public bool TeamOwner { get; set; }
        [SerializedName("pickTurn")] public int PickTurn { get; set; }
        [SerializedName("pickMode")] public int PickMode { get; set; }
        [SerializedName("clientInSynch")] public bool ClientInSynch { get; set; }
        [SerializedName("queueRating")] public int QueueRating { get; set; }
        [SerializedName("botDifficulty")] public string BotDifficulty { get; set; }
        [SerializedName("minor")] public bool Minor { get; set; }
        [SerializedName("lastSelectedSkinIndex")] public int LastSelectedSkinIndex { get; set; }
        [SerializedName("partnerId")] public string PartnerId { get; set; }
        [SerializedName("rankedTeamGuest")] public bool RankedTeamGuest { get; set; }
        [SerializedName("voterRating")] public int VoterRating { get; set; }
        [SerializedName("dataVersion")] public int DataVersion { get; set; }
        [SerializedName("index")] public int Index { get; set; }
        [SerializedName("originalAccountNumber")] public double OriginalAccountNumber { get; set; }
        [SerializedName("adjustmentFlags")] public double AdjustmentFlags { get; set; }
        [SerializedName("badges")] public int Badges { get; set; }
        [SerializedName("teamRating")] public int TeamRating { get; set; }
        [SerializedName("originalPlatformId")] public string OriginalPlatformId { get; set; }
    }
}