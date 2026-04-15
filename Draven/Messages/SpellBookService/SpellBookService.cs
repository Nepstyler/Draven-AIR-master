using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Draven.ServerModels;
using Draven.Structures;
using Draven.Structures.Platform.Summoner;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;

namespace Draven.Messages.SpellBookService
{
    class GetSpellBook : IMessage
    {
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;
            double summonerId = client._sumId;

            SpellBookDTO book = new SpellBookDTO
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
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM rune_pages WHERE account_id = @accId", conn))
                    {
                        cmd.Parameters.AddWithValue("@accId", summonerId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SpellBookPageDTO page = new SpellBookPageDTO();
                                page.PageId = reader.GetInt32("id");
                                page.Name = reader.GetString("page_name");
                                page.Current = reader.GetBoolean("is_current");
                                page.SummonerId = summonerId;
                                page.SlotEntries = new ArrayCollection();

                                for (int i = 1; i <= 30; i++)
                                {
                                    int runeId = reader.GetInt32("slot_" + i);
                                    if (runeId > 0)
                                    {
                                        page.SlotEntries.Add(new SlotEntry { RuneSlotId = i, RuneId = runeId });
                                    }
                                }
                                book.BookPages.Add(page);
                            }
                        }
                    }

                    if (book.BookPages.Count == 0)
                    {
                        book.BookPages.Add(new SpellBookPageDTO { PageId = 1, Name = "Rune Page 1", Current = true, SummonerId = summonerId, SlotEntries = new ArrayCollection() });
                        book.BookPages.Add(new SpellBookPageDTO { PageId = 2, Name = "Rune Page 2", Current = false, SummonerId = summonerId, SlotEntries = new ArrayCollection() });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("[ERROR] Eroare DB GetSpellBook: " + ex.Message); }

            e.ReturnRequired = true;
            e.Data = book;
            return e;
        }
    }
}