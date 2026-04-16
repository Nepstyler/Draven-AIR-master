using System;
using MySql.Data.MySqlClient;
using Draven.ServerModels;
using Draven.Structures.Platform.Summoner;
using RtmpSharp.Messaging;

namespace Draven.Messages.SummonerService
{
    class GetSummonerByName : IMessage
    {
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            // Clientul trimite numele cerut intr-un Array de obiecte (Body)
            object[] bodyArgs = e.Body as object[];
            string requestedName = "";

            if (bodyArgs != null && bodyArgs.Length > 0)
            {
                requestedName = bodyArgs[0].ToString();
            }

            Summoner targetSummoner = null;

            if (!string.IsNullOrEmpty(requestedName))
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();
                        // Cautam contul exact dupa username
                        using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM accounts WHERE username = @uname LIMIT 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@uname", requestedName);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    targetSummoner = new Summoner
                                    {
                                        AcctId = reader.GetInt32("id"),
                                        SumId = reader.GetInt32("summonerId"),
                                        Name = reader.GetString("username"),
                                        InternalName = reader.GetString("username").ToLower(),
                                        ProfileIconId = 1, // Iconita default, poti citi din db mai tarziu
                                        RevisionDate = DateTime.Now,
                                        RevisionId = 0
                                    };
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] DB la GetSummonerByName: " + ex.Message);
                }
            }

            e.ReturnRequired = true;
            e.Data = targetSummoner;
            return e;
        }
    }
}