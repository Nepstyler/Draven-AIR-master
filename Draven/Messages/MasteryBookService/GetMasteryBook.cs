using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Draven.ServerModels;
using Draven.Structures;
using Draven.Structures.Platform.Summoner;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;

namespace Draven.Messages.MasteryBookService
{
    class GetMasteryBook : IMessage
    {
        // Conexiunea la baza de date lol
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient summonerSender = sender as SummonerClient;
            double summonerId = summonerSender._sumId;

            MasteryBookDTO MasteryBook = new MasteryBookDTO
            {
                SummonerId = summonerId,
                DateString = DateTime.Now.ToString(),
                BookPages = new ArrayCollection()
            };

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM mastery_pages WHERE account_id = @accId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@accId", summonerId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MasteryBookPageDTO page = new MasteryBookPageDTO();
                                page.PageId = reader.GetInt32("id");
                                page.Name = reader.GetString("page_name");
                                page.Current = reader.GetBoolean("is_current");
                                page.SummonerId = summonerId;
                                page.Entries = new ArrayCollection();

                                string json = reader.GetString("talents_json");
                                if (!string.IsNullOrEmpty(json) && json != "{}")
                                {
                                    Dictionary<int, int> talents = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
                                    foreach (KeyValuePair<int, int> kvp in talents)
                                    {
                                        TalentEntry entry = new TalentEntry();
                                        entry.TalentId = kvp.Key;
                                        entry.Rank = kvp.Value;
                                        entry.SummonerId = summonerId;
                                        page.Entries.Add(entry);
                                    }
                                }
                                MasteryBook.BookPages.Add(page);
                            }
                        }
                    }

                    if (MasteryBook.BookPages.Count == 0)
                    {
                        MasteryBookPageDTO defaultPage = new MasteryBookPageDTO();
                        defaultPage.Current = true;
                        defaultPage.SummonerId = summonerId;
                        defaultPage.PageId = 1;
                        defaultPage.Name = "Mastery Page 1";
                        defaultPage.Entries = new ArrayCollection();
                        MasteryBook.BookPages.Add(defaultPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Eroare DB MasteryBook: " + ex.Message);
            }

            e.ReturnRequired = true;
            e.Data = MasteryBook;
            return e;
        }
    }
}