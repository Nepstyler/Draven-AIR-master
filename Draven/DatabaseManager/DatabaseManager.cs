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
            Dictionary<string, int> _masterySort = new Dictionary<string, int> { { "Ferocity", 1 }, { "Cunning", 2 }, { "Resolve", 3 } };

            Console.WriteLine("[LOG] Initialize Mastery and Rune Tree");
            using (WebClient client = new WebClient())
            {
                //Download the latest mastery daata
                string MasteryData = client.DownloadString("http://ddragon.leagueoflegends.com/cdn/6.24.1/data/en_US/mastery.json");

                Masteries mData = JsonConvert.DeserializeObject<Masteries>(MasteryData);
                TalentTree = new ArrayCollection();

                foreach (var mastery in mData.tree)
                {
                    TalentGroup group = new TalentGroup
                    {
                        Name = mastery.Key,
                        TalentRows = new ArrayCollection(),
                        TltGroupId = _masterySort[mastery.Key],
                        Index = _masterySort[mastery.Key] - 1
                    };

                    for (int i = 0; i < mastery.Value.Count; i++)
                    {
                        ArrayCollection talentList = new ArrayCollection();
                        List<MasteryLite> masteryList = mastery.Value[i];
                        for (int j = 0; j < masteryList.Count; j++)
                        {
                            if (masteryList[j] == null)
                                continue;

                            var data = mData.data[masteryList[j].masteryId];
                            Talent t = new Talent
                            {
                                Index = j,
                                Name = data.name,
                                Level1Desc = data.name,
                                Level2Desc = data.name,
                                Level3Desc = data.name,
                                Level4Desc = data.name,
                                Level5Desc = data.name,
                                GameCode = data.id,
                                TltId = data.id,
                                MaxRank = data.ranks,
                                MinLevel = 1,
                                MinTier = 1,
                                TalentGroupId = group.TltGroupId,
                                TalentRowId = (i + 1) * group.TltGroupId
                            };

                            if (data.preReq != "0")
                                t.PrereqTalentGameCode = Convert.ToInt32(data.preReq);

                            talentList.Add(t);
                        }

                        TalentRow row = new TalentRow
                        {
                            Index = i,
                            Talents = talentList,
                            PointsToActivate = i * 4,
                            TltRowId = (i + 1) * group.TltGroupId,
                            TltGroupId = group.TltGroupId
                        };

                        group.TalentRows.Add(row);
                    }

                    TalentTree.Add(group);
                }

                #region Rune Loading
                RuneTree = new ArrayCollection();

                int Modifier = 0;
                int Take = 3;
                for (int i = 1; i <= 9; i++)
                {
                    if ((i - 1) % 3 == 0 && i != 1)
                    {
                        Modifier += 1;
                    }

                    int IdAdd = (Math.Abs(Take - 3) * 10);
                    IdAdd -= Math.Abs(Take - 3);

                    RuneSlot slot = new RuneSlot()
                    {
                        Id = IdAdd + i,
                        RuneType = new RuneType(),
                        MinLevel = (3 * i + 1) - Take + Modifier
                    };

                    if (Take == 3)
                    {
                        slot.RuneType.Name = "Red";
                        slot.RuneType.Id = 1;
                    }
                    else if (Take == 2)
                    {
                        slot.RuneType.Name = "Yellow";
                        slot.RuneType.Id = 3;
                    }
                    else
                    {
                        slot.RuneType.Name = "Blue";
                        slot.RuneType.Id = 5;
                    }

                    RuneTree.Add(slot);

                    if (i == 9 && Take > 1)
                    {
                        Take -= 1;
                        i = 0;
                        Modifier = 0;
                    }
                }

                for (int i = 1; i <= 3; i++)
                {
                    RuneSlot slot = new RuneSlot()
                    {
                        Id = 27 + i,
                        RuneType = new RuneType
                        {
                            Id = 7,
                            Name = "Black"
                        },
                        MinLevel = 10 * i
                    };

                    RuneTree.Add(slot);
                }
                #endregion
            }
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