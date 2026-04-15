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
    class SaveMasteryBook : IMessage
    {
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            object[] parameters = e.Body as object[];
            if (parameters == null || parameters.Length == 0) return e;

            MasteryBookDTO masteryBook = parameters[0] as MasteryBookDTO;

            if (masteryBook != null)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();

                        using (MySqlCommand delCmd = new MySqlCommand("DELETE FROM mastery_pages WHERE account_id = @accId", conn))
                        {
                            delCmd.Parameters.AddWithValue("@accId", masteryBook.SummonerId);
                            delCmd.ExecuteNonQuery();
                        }

                        foreach (object pageObj in masteryBook.BookPages)
                        {
                            MasteryBookPageDTO page = pageObj as MasteryBookPageDTO;
                            if (page == null) continue;

                            Dictionary<int, int> talentsDict = new Dictionary<int, int>();
                            if (page.Entries != null)
                            {
                                foreach (object entryObj in page.Entries)
                                {
                                    TalentEntry entry = entryObj as TalentEntry;
                                    if (entry != null)
                                    {
                                        talentsDict[entry.TalentId] = entry.Rank;
                                    }
                                }
                            }

                            string jsonTalents = JsonConvert.SerializeObject(talentsDict);

                            string query = "INSERT INTO mastery_pages (account_id, page_name, is_current, talents_json) VALUES (@accId, @name, @current, @json)";
                            using (MySqlCommand insCmd = new MySqlCommand(query, conn))
                            {
                                insCmd.Parameters.AddWithValue("@accId", masteryBook.SummonerId);
                                insCmd.Parameters.AddWithValue("@name", page.Name);
                                insCmd.Parameters.AddWithValue("@current", page.Current ? 1 : 0);
                                insCmd.Parameters.AddWithValue("@json", jsonTalents);
                                insCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    Console.WriteLine("[SUCCESS] Paginile de Măiestrii au fost salvate pentru: " + client._session.Summary.Username);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Eroare DB SaveMasteryBook: " + ex.Message);
                }
            }

            e.ReturnRequired = true;
            e.Data = masteryBook;
            return e;
        }
    }
}