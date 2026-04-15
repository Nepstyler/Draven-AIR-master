namespace Draven.Structures.Platform.Game
{
    using System;
    using RtmpSharp.IO;
    using RtmpSharp.IO.AMF3;

    [Serializable]
    [SerializedName("com.riotgames.platform.game.GameDTO")]
    public class GameDTO
    {
        [SerializedName("id")] public double Id { get; set; }
        [SerializedName("name")] public string Name { get; set; }
        [SerializedName("roomName")] public string RoomName { get; set; }
        [SerializedName("roomPassword")] public string RoomPassword { get; set; }
        [SerializedName("gameState")] public string GameState { get; set; }
        [SerializedName("gameStateString")] public string GameStateString { get; set; }
        [SerializedName("gameMode")] public string GameMode { get; set; }
        [SerializedName("gameType")] public string GameType { get; set; }
        [SerializedName("gameTypeConfigId")] public int GameTypeConfigId { get; set; }
        [SerializedName("mapId")] public int MapId { get; set; }
        [SerializedName("maxNumPlayers")] public int MaxNumPlayers { get; set; }
        [SerializedName("queueTypeName")] public string QueueTypeName { get; set; }
        [SerializedName("queuePosition")] public int QueuePosition { get; set; }
        [SerializedName("optimisticLock")] public double OptimisticLock { get; set; }
        [SerializedName("pickTurn")] public int PickTurn { get; set; }
        [SerializedName("expiryTime")] public double ExpiryTime { get; set; }
        [SerializedName("joinTimerDuration")] public int JoinTimerDuration { get; set; }
        [SerializedName("spectatorsAllowed")] public string SpectatorsAllowed { get; set; }
        [SerializedName("terminatedCondition")] public string TerminatedCondition { get; set; }
        [SerializedName("terminatedConditionString")] public string TerminatedConditionString { get; set; }

        [SerializedName("ownerSummary")] public PlayerParticipant OwnerSummary { get; set; }
        [SerializedName("teamOne")] public ArrayCollection TeamOne { get; set; }
        [SerializedName("teamTwo")] public ArrayCollection TeamTwo { get; set; }
        [SerializedName("observers")] public ArrayCollection Observers { get; set; }
        [SerializedName("bannedChampions")] public ArrayCollection BannedChampions { get; set; }
        [SerializedName("banOrder")] public ArrayCollection BanOrder { get; set; }
        [SerializedName("playerChampionSelections")] public ArrayCollection PlayerChampionSelections { get; set; }
        [SerializedName("gameMutators")] public ArrayCollection GameMutators { get; set; }
        [SerializedName("practiceGameRewardsDisabledReasons")] public ArrayCollection PracticeGameRewardsDisabledReasons { get; set; }

        [SerializedName("featuredGameInfo")] public FeaturedGameInfo FeaturedGameInfo { get; set; }
    }
}