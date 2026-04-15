using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Draven.Helpers
{
    // The exact root structure required by GameServerConsole
    public class GameInfoRoot
    {
        [JsonProperty("players")]
        public List<GameInfoPlayer> Players { get; set; }

        [JsonProperty("game")]
        public GameSettings Game { get; set; }

        [JsonProperty("gameInfo")]
        public GameServerConfig GameInfo { get; set; }

        [JsonProperty("forcedStart")]
        public int ForcedStart { get; set; }
    }

    public class GameInfoPlayer
    {
        public int playerId { get; set; }
        public string blowfishKey { get; set; }
        public string rank { get; set; }
        public string name { get; set; }
        public string champion { get; set; }
        public string team { get; set; }
        public int skin { get; set; }
        public string summoner1 { get; set; }
        public string summoner2 { get; set; }
        public int ribbon { get; set; }
        public int icon { get; set; }
        public Dictionary<string, int> runes { get; set; }
        public Dictionary<string, int> talents { get; set; }
    }

    public class GameSettings
    {
        [JsonProperty("map")]
        public int Map { get; set; }

        [JsonProperty("gameMode")]
        public string GameMode { get; set; }

        [JsonProperty("dataPackage")]
        public string DataPackage { get; set; }
    }

    public class GameServerConfig
    {
        public bool MANACOSTS_ENABLED { get; set; }
        public bool COOLDOWNS_ENABLED { get; set; }
        public bool CHEATS_ENABLED { get; set; }
        public bool MINION_SPAWNS_ENABLED { get; set; }
        public string CONTENT_PATH { get; set; }
        public bool IS_DAMAGE_TEXT_GLOBAL { get; set; }
    }

    public static class ConstantsTranslator
    {
        public static string GetSpellName(int id)
        {
            switch (id)
            {
                case 1: return "SummonerBoost";
                case 3: return "SummonerExhaust";
                case 4: return "SummonerFlash";
                case 6: return "SummonerHaste";
                case 7: return "SummonerHeal";
                case 11: return "SummonerSmite";
                case 12: return "SummonerTeleport";
                case 13: return "SummonerMana";
                case 14: return "SummonerDot";
                case 21: return "SummonerBarrier";
                default: return "SummonerHeal";
            }
        }

        // Translation based on Magic Numbers documentation
        public static string GetChampionName(int id)
        {
            switch (id)
            {
                case 157: return "Yasuo";
                case 266: return "Aatrox";
                case 103: return "Ahri";
                case 84: return "Akali";
                case 12: return "Alistar";
                case 32: return "Amumu";
                case 1: return "Annie";
                case 22: return "Ashe";
                default: return "Yasuo";
            }
        }
    }
}