using Draven.ServerModels;
using Draven.Structures;

using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Draven.Messages.SummonerService
{
    using Draven.Structures.Platform.Summoner;
    using Newtonsoft.Json;

    class GetAllSummonerDataByAccount : IMessage
    {
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            object[] body = e.Body as object[];
            SummonerClient summonerSender = sender as SummonerClient;
            int creds = Convert.ToInt32(body[0]);

            // PRELUĂM RUNELE REALE DIN BAZA DE DATE
            SpellBookDTO dbSpellBook = new SpellBookDTO
            {
                SummonerId = summonerSender._sumId,
                DateString = DateTime.Now.ToString(),
                BookPages = new ArrayCollection()
            };

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM rune_pages WHERE account_id = @accId", conn))
                    {
                        cmd.Parameters.AddWithValue("@accId", summonerSender._sumId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SpellBookPageDTO page = new SpellBookPageDTO();
                                page.PageId = reader.GetInt32("id"); // INT curat, a rezolvat eroarea!
                                page.Name = reader.GetString("page_name");
                                page.Current = reader.GetBoolean("is_current");
                                page.SummonerId = summonerSender._sumId;
                                page.CreateDate = DateTime.Now;
                                page.SlotEntries = new ArrayCollection();

                                for (int i = 1; i <= 30; i++)
                                {
                                    int runeId = reader.GetInt32("slot_" + i);
                                    if (runeId > 0)
                                    {
                                        page.SlotEntries.Add(new SlotEntry { RuneSlotId = i, RuneId = runeId });
                                    }
                                }
                                dbSpellBook.BookPages.Add(page);
                            }
                        }
                    }
                    if (dbSpellBook.BookPages.Count == 0)
                    {
                        dbSpellBook.BookPages.Add(new SpellBookPageDTO { PageId = 1, Name = "Rune Page 1", Current = true, SummonerId = summonerSender._sumId, CreateDate = DateTime.Now, SlotEntries = new ArrayCollection() });
                        dbSpellBook.BookPages.Add(new SpellBookPageDTO { PageId = 2, Name = "Rune Page 2", Current = false, SummonerId = summonerSender._sumId, CreateDate = DateTime.Now, SlotEntries = new ArrayCollection() });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("[ERROR] DB Profile Rune: " + ex.Message); }

            AllSummonerData allSD = new AllSummonerData()
            {
                SummonerDefaultSpells = new SummonerDefaultSpells()
                {
                    SummonerId = summonerSender._sumId,
                    SpellDefault1 = new ArrayCollection(),
                    SpellDefault2 = new ArrayCollection()
                },
                SummonerTalentsAndPoints = new SummonerTalentsAndPoints()
                {
                    TalentPoints = 30,
                    ModifyDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    CreateDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    SummonerId = summonerSender._sumId
                },
                Summoner = new Summoner()
                {
                    InternalName = summonerSender._session.Summary.Username,
                    PreviousSeasonHighestTier = "CHALLENGER",
                    PreviousSeasonHighestTeamReward = 5,
                    AcctId = summonerSender._accId,
                    HelpFlag = false,
                    SumId = summonerSender._sumId,
                    ProfileIconId = Convert.ToInt32(summonerSender._sumIcon),
                    DisplayEloQuestionaire = false,
                    LastGameDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    RevisionDate = new DateTime(2016, 08, 11, 12, 00, 00),
                    AdvancedTutorialFlag = false,
                    RevisionId = 1,
                    Name = summonerSender._summonername,
                    NameChangeFlag = false,
                    TutorialFlag = false,
                    SummonerId = summonerSender._sumId
                },
                SummonerLevel = new SummonerLevel
                {
                    ExpTierMod = 1.0,
                    SummonerTier = 5.0,
                    InfTierMod = 1.0,
                    ExpToNextLevel = 32651,
                    Level = 30.0
                },
                SummonerLevelAndPoints = new SummonerLevelAndPoints
                {
                    InfPoints = 0,
                    ExpPoints = 32651,
                    SummonerId = summonerSender._sumId,
                    SummonerLevel = 30
                },
                // Atribuim SpellBook-ul preluat din baza de date
                SpellBook = dbSpellBook
            };

            // Nu avem nevoie sa spammam consola cu json-ul profilului
            // Console.WriteLine(JsonConvert.SerializeObject(allSD));

            e.ReturnRequired = true;
            e.Data = allSD;
            return e;
        }
    }
}