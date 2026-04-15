using System;

namespace Draven.Messages.GameService
{
    // This class acts as a temporary memory for the emulator
    // to remember what the user selected during Champion Select.
    public static class LobbyState
    {
        public static int ChampionId = 0;
        public static int SkinIndex = 0;
        public static int Spell1Id = 7; // Default Heal
        public static int Spell2Id = 4; // Default Flash

        // Helper to convert Spell IDs to the internal string names expected by GameInfo.json
        public static string GetSpellName(int id)
        {
            switch (id)
            {
                case 1: return "SummonerBoost"; // Cleanse
                case 3: return "SummonerExhaust";
                case 4: return "SummonerFlash";
                case 6: return "SummonerHaste"; // Ghost
                case 7: return "SummonerHeal";
                case 11: return "SummonerSmite";
                case 12: return "SummonerTeleport";
                case 13: return "SummonerMana"; // Clarity
                case 14: return "SummonerDot"; // Ignite
                case 21: return "SummonerBarrier";
                default: return "SummonerHeal";
            }
        }
    }
}