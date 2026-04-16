namespace Draven.DatabaseManager
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Net;

    using Draven.Structures.Platform.Catalog;
    using Draven.Structures.Platform.Summoner;

    using MySql.Data.MySqlClient;

    using Newtonsoft.Json;

    using RtmpSharp.IO.AMF3;

    public static class DatabaseManager
    {
        public static ArrayCollection TalentTree { get; set; }
        public static ArrayCollection RuneTree { get; set; }

        public static MySqlConnection connection { get; set; }
        public static MySqlDataReader rdr = null;
        public static object Locker = new object();

        public static bool InitConnection()
        {
            try
            {
                Console.WriteLine("[LOG] Connecting to database");
                connection = new MySqlConnection("Database=" + Program.database + ";Data Source=" + Program.host + ";User Id = " + Program.user + "; Password = " + Program.pass + "; SslMode=none");
                connection.Open();
                Console.WriteLine("[LOG] Connection established");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[LOG] Couldn't connect to database.\n" + e.Message);
                return false;
            }
        }

        // --- Metode Noi pentru Lobby ---
        public static int CreateLobby(int ownerId, string name, int mapId, string mode)
        {
            lock (Locker)
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO lobbies (name, ownerId, mapId, gameMode) VALUES (@name, @owner, @map, @mode); SELECT LAST_INSERT_ID();";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@owner", ownerId);
                cmd.Parameters.AddWithValue("@map", mapId);
                cmd.Parameters.AddWithValue("@mode", mode);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void AddParticipantToLobby(int lobbyId, int summonerId, int team)
        {
            lock (Locker)
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO lobby_participants (lobbyId, summonerId, team) VALUES (@lobby, @sum, @team) ON DUPLICATE KEY UPDATE team=@team";
                cmd.Parameters.AddWithValue("@lobby", lobbyId);
                cmd.Parameters.AddWithValue("@sum", summonerId);
                cmd.Parameters.AddWithValue("@team", team);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateChampionSelection(int lobbyId, int summonerId, int championId)
        {
            lock (Locker)
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE lobby_participants SET pickId=@pick WHERE lobbyId=@lobby AND summonerId=@sum";
                cmd.Parameters.AddWithValue("@pick", championId);
                cmd.Parameters.AddWithValue("@lobby", lobbyId);
                cmd.Parameters.AddWithValue("@sum", summonerId);
                cmd.ExecuteNonQuery();
            }
        }
        // --- Sfarsit Metode Noi ---

        public static void InitMasteryAndRuneTree()
        {
            Console.WriteLine("[LOG] Initialize Mastery and Rune Tree (100% Original S4 GitHub Data)");

            TalentTree = new RtmpSharp.IO.AMF3.ArrayCollection();

            string[] groupNames = new string[] { "Offense", "Defense", "Utility" };
            for (int i = 0; i < 3; i++)
            {
                TalentTree.Add(new Draven.Structures.Platform.Summoner.TalentGroup
                {
                    Name = groupNames[i],
                    TalentRows = new RtmpSharp.IO.AMF3.ArrayCollection(),
                    TltGroupId = i + 1,
                    Index = i
                });
            }

            Action<int, int, int> addRow = (groupId, rowIndex, pointsReq) => {
                ((Draven.Structures.Platform.Summoner.TalentGroup)TalentTree[groupId - 1]).TalentRows.Add(new Draven.Structures.Platform.Summoner.TalentRow
                {
                    Index = rowIndex,
                    Talents = new RtmpSharp.IO.AMF3.ArrayCollection(),
                    PointsToActivate = pointsReq,
                    TltRowId = ((groupId - 1) * 6) + rowIndex + 1,
                    TltGroupId = groupId
                });
            };

            // Funcția care generează ID-ul corect matematic ca la Riot: 4000 + (Arbore*100) + (Rând*10) + Coloană
            Action<int, int, int, int, int> addTalent = (groupId, rowIndex, colIndex, maxRank, prereqColIndex) => {
                var row = (Draven.Structures.Platform.Summoner.TalentRow)((Draven.Structures.Platform.Summoner.TalentGroup)TalentTree[groupId - 1]).TalentRows[rowIndex];

                int gameCode = 4000 + (groupId * 100) + ((rowIndex + 1) * 10) + (colIndex + 1);

                var t = new Draven.Structures.Platform.Summoner.Talent
                {
                    Index = colIndex,
                    GameCode = gameCode,
                    TltId = gameCode,
                    MinLevel = 1,
                    MinTier = 1,
                    MaxRank = maxRank,
                    TalentGroupId = groupId,
                    TalentRowId = row.TltRowId,
                    Name = "Tlt" + gameCode,
                    Level1Desc = "-",
                    Level2Desc = "-",
                    Level3Desc = "-",
                    Level4Desc = "-",
                    Level5Desc = "-"
                };

                // Dacă are prereq, se calculează automat ID-ul măiestriei de deasupra ei!
                if (prereqColIndex != -1)
                {
                    t.PrereqTalentGameCode = 4000 + (groupId * 100) + (rowIndex * 10) + (prereqColIndex + 1);
                }

                row.Talents.Add(t);
            };

            // ================== OFFENSE TREE ==================
            addRow(1, 0, 0);
            addTalent(1, 0, 0, 1, -1); addTalent(1, 0, 1, 4, -1); addTalent(1, 0, 2, 4, -1); addTalent(1, 0, 3, 1, -1);
            addRow(1, 1, 4);
            addTalent(1, 1, 0, 1, -1); addTalent(1, 1, 1, 3, -1); addTalent(1, 1, 2, 3, -1); addTalent(1, 1, 3, 1, 3);
            addRow(1, 2, 8);
            addTalent(1, 2, 0, 1, -1); addTalent(1, 2, 1, 1, 1); addTalent(1, 2, 2, 1, 2); addTalent(1, 2, 3, 3, -1);
            addRow(1, 3, 12);
            addTalent(1, 3, 0, 1, 0); addTalent(1, 3, 1, 3, -1); addTalent(1, 3, 2, 3, -1); addTalent(1, 3, 3, 1, 3);
            addRow(1, 4, 16);
            addTalent(1, 4, 0, 1, -1); addTalent(1, 4, 1, 3, -1); addTalent(1, 4, 3, 1, -1);
            addRow(1, 5, 20);
            addTalent(1, 5, 1, 1, -1);

            // ================== DEFENSE TREE (Identic cu data.js original din S4) ==================
            addRow(2, 0, 0); // Linia 0
            addTalent(2, 0, 0, 2, -1); // Block
            addTalent(2, 0, 1, 2, -1); // Recovery
            addTalent(2, 0, 2, 2, -1); // Enchanted Armor
            addTalent(2, 0, 3, 2, -1); // Tough Skin

            addRow(2, 1, 4); // Linia 1
            addTalent(2, 1, 0, 1, 0);  // Unyielding (din Block col 0)
            addTalent(2, 1, 1, 3, -1); // Veteran Scars
            addTalent(2, 1, 3, 1, 3);  // Bladed Armor (din Tough Skin col 3)

            addRow(2, 2, 8); // Linia 2
            addTalent(2, 2, 0, 1, -1); // Oppression
            addTalent(2, 2, 1, 1, 1);  // Juggernaut (din Veteran Scars col 1)
            addTalent(2, 2, 2, 3, -1); // Hardiness
            addTalent(2, 2, 3, 3, -1); // Resistance

            addRow(2, 3, 12); // Linia 3
            addTalent(2, 3, 0, 3, -1); // Perseverance
            addTalent(2, 3, 1, 1, -1); // Swiftness
            addTalent(2, 3, 2, 1, 2);  // Reinforced Armor (din Hardiness col 2)
            addTalent(2, 3, 3, 1, 3);  // Evasive (din Resistance col 3)

            addRow(2, 4, 16); // Linia 4
            addTalent(2, 4, 0, 1, -1); // Second Wind
            addTalent(2, 4, 1, 4, -1); // Legendary Guardian
            addTalent(2, 4, 2, 1, -1); // Runic Blessing

            addRow(2, 5, 20); // Linia 5
            addTalent(2, 5, 1, 1, -1); // Tenacious

            // ================== UTILITY TREE (Identic cu data.js original din S4) ==================
            addRow(3, 0, 0); // Linia 0
            addTalent(3, 0, 0, 1, -1); // Phasewalk
            addTalent(3, 0, 1, 3, -1); // Fleet of Foot
            addTalent(3, 0, 2, 3, -1); // Meditation
            addTalent(3, 0, 3, 1, -1); // Scout

            addRow(3, 1, 4); // Linia 1
            addTalent(3, 1, 1, 3, -1); // Summoner's Insight
            addTalent(3, 1, 2, 1, 2);  // Strength of Spirit (din Meditation col 2)
            addTalent(3, 1, 3, 1, -1); // Alchemist

            addRow(3, 2, 8); // Linia 2
            addTalent(3, 2, 0, 3, -1); // Greed
            addTalent(3, 2, 1, 1, -1); // Runic Affinity
            addTalent(3, 2, 2, 1, -1); // Vampirism
            addTalent(3, 2, 3, 1, 3);  // Culinary Master (din Alchemist col 3)

            addRow(3, 3, 12); // Linia 3
            addTalent(3, 3, 0, 1, 0);  // Scavenger (din Greed col 0)
            addTalent(3, 3, 1, 1, -1); // Wealth
            addTalent(3, 3, 2, 3, -1); // Expanded Mind
            addTalent(3, 3, 3, 2, -1); // Inspiration

            addRow(3, 4, 16); // Linia 4
            addTalent(3, 4, 1, 1, 1);  // Bandit (din Wealth col 1)
            addTalent(3, 4, 2, 3, -1); // Intelligence

            addRow(3, 5, 20); // Linia 5
            addTalent(3, 5, 1, 1, -1); // Nimble/Wanderer

            #region Rune Loading
            RuneTree = new RtmpSharp.IO.AMF3.ArrayCollection();
            int Modifier = 0; int Take = 3;
            for (int i = 1; i <= 9; i++)
            {
                if ((i - 1) % 3 == 0 && i != 1) Modifier += 1;
                int IdAdd = (Math.Abs(Take - 3) * 10) - Math.Abs(Take - 3);
                Draven.Structures.Platform.Catalog.RuneSlot slot = new Draven.Structures.Platform.Catalog.RuneSlot() { Id = IdAdd + i, RuneType = new Draven.Structures.Platform.Catalog.RuneType(), MinLevel = (3 * i + 1) - Take + Modifier };
                if (Take == 3) { slot.RuneType.Name = "Red"; slot.RuneType.Id = 1; } else if (Take == 2) { slot.RuneType.Name = "Yellow"; slot.RuneType.Id = 3; } else { slot.RuneType.Name = "Blue"; slot.RuneType.Id = 5; }
                RuneTree.Add(slot);
                if (i == 9 && Take > 1) { Take -= 1; i = 0; Modifier = 0; }
            }
            for (int i = 1; i <= 3; i++) RuneTree.Add(new Draven.Structures.Platform.Catalog.RuneSlot() { Id = 27 + i, RuneType = new Draven.Structures.Platform.Catalog.RuneType { Id = 7, Name = "Black" }, MinLevel = 10 * i });
            #endregion
        }

        public static List<int> ProfileIcons = new List<int>();

        public static void InitProfileIcons()
        {
            using (WebClient client = new WebClient())
            {
                Console.WriteLine("[LOG] Initialize Profile Icons");
                string ProfileData = client.DownloadString("http://ddragon.leagueoflegends.com/cdn/7.12.1/data/en_US/profileicon.json");
                ProfileJsonTree mData = JsonConvert.DeserializeObject<ProfileJsonTree>(ProfileData);

                foreach (var iconData in mData.data)
                {
                    ProfileIcons.Add(iconData.Value.id);
                }
            }
        }

        public static Dictionary<string, string> getAccountData(string user, string pass)
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM accounts WHERE username='" + user + "' AND password='" + pass + "'";
            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dtCustomers = new DataTable();
            dtCustomers.Load(reader);
            var dataArray = new Dictionary<string, string>();
            foreach (DataRow row in dtCustomers.Rows)
            {
                dataArray["id"] = row["id"].ToString();
                dataArray["summonerId"] = row["summonerId"].ToString();
                dataArray["RP"] = row["RP"].ToString();
                dataArray["IP"] = row["IP"].ToString();
                dataArray["banned"] = row["isBanned"].ToString();
            }
            return dataArray;
        }

        public static Dictionary<string, string> getSummonerData(string sumId)
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM summoner WHERE id='" + sumId + "'";
            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dtCustomers = new DataTable();
            dtCustomers.Load(reader);
            var dataArray = new Dictionary<string, string>();
            foreach (DataRow row in dtCustomers.Rows)
            {
                dataArray["id"] = row["id"].ToString();
                dataArray["summonerName"] = row["summonerName"].ToString();
                dataArray["icon"] = row["icon"].ToString();
            }
            return dataArray;
        }

        public class DBChampions
        {
            public int ID { get; set; }
            public bool IsFreeToPlay { get; set; }
        }

        public static List<DBChampions> getAllChampions()
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM champions";
            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dtChampions = new DataTable();
            dtChampions.Load(reader);
            var dataArray = new List<DBChampions>();
            foreach (DataRow row in dtChampions.Rows)
            {
                dataArray.Add(new DBChampions() { ID = Convert.ToInt32(row["id"].ToString()), IsFreeToPlay = Convert.ToBoolean(Convert.ToInt32(row["freeToPlay"].ToString())) });
            }
            return dataArray;
        }

        public static List<int> getAllChampionSkinsForId(int id)
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM championSkins WHERE championId='" + id + "'";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                DataTable dtSkins = new DataTable();
                dtSkins.Load(reader);
                var dataArray = new List<int>();
                foreach (DataRow row in dtSkins.Rows)
                {
                    int skinId = Convert.ToInt32(row["id"].ToString());
                    // Prevent the client from receiving the base skin (ending in 000) twice
                    if (skinId % 1000 != 0)
                    {
                        dataArray.Add(skinId);
                    }
                }
                return dataArray;
            }
        }

        public static void updateSummonerIconById(int sumId, int iconId)
        {
            try
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE summoner SET icon='" + iconId + "' WHERE id='" + sumId + "'";
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException sex)
            {
                Console.WriteLine(sex.Message);
            }
        }

        public static bool checkAccount(string user, string pass)
        {
            try
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM accounts WHERE username='" + user + "' AND password='" + pass + "'";
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                return userCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}