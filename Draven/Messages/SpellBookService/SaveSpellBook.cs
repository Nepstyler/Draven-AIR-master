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
    class SaveSpellBook : IMessage
    {
        private string connString = "Server=127.0.0.1;Database=lol;Uid=root;Pwd=;";

        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            object[] parameters = e.Body as object[];
            if (parameters == null || parameters.Length == 0) return e;

            SpellBookDTO spellBook = parameters[0] as SpellBookDTO;

            if (spellBook != null)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();

                        // Ștergem paginile vechi
                        using (MySqlCommand delCmd = new MySqlCommand("DELETE FROM rune_pages WHERE account_id = @accId", conn))
                        {
                            delCmd.Parameters.AddWithValue("@accId", spellBook.SummonerId);
                            delCmd.ExecuteNonQuery();
                        }

                        // Salvăm paginile noi, inclusiv itemele
                        foreach (object pageObj in spellBook.BookPages)
                        {
                            SpellBookPageDTO page = pageObj as SpellBookPageDTO;
                            if (page == null) continue;

                            int[] slots = new int[31];
                            if (page.SlotEntries != null)
                            {
                                foreach (object entryObj in page.SlotEntries)
                                {
                                    int slotId = 0;
                                    int runeId = 0;

                                    // Metoda 1: Daca obiectul este clar SlotEntry
                                    if (entryObj is SlotEntry entry)
                                    {
                                        slotId = entry.RuneSlotId;
                                        runeId = entry.RuneId;
                                    }
                                    // Metoda 2: Daca obiectul vine ca AsObject (AMF Dictionary netipat)
                                    else if (entryObj is RtmpSharp.IO.AsObject asObj)
                                    {
                                        if (asObj.ContainsKey("runeSlotId")) slotId = Convert.ToInt32(asObj["runeSlotId"]);
                                        if (asObj.ContainsKey("runeId")) runeId = Convert.ToInt32(asObj["runeId"]);
                                    }
                                    // Metoda 3: Metoda de siguranta prin dynamic/reflexie
                                    else
                                    {
                                        try
                                        {
                                            dynamic dyn = entryObj;
                                            slotId = Convert.ToInt32(dyn.runeSlotId);
                                            runeId = Convert.ToInt32(dyn.runeId);
                                        }
                                        catch { }
                                    }

                                    // Adaugam in vector doar daca indexul si runa sunt valide
                                    if (slotId >= 1 && slotId <= 30 && runeId > 0)
                                    {
                                        slots[slotId] = runeId;
                                    }
                                }
                            }

                            string insertQuery = @"INSERT INTO rune_pages (account_id, page_name, is_current, 
                                slot_1, slot_2, slot_3, slot_4, slot_5, slot_6, slot_7, slot_8, slot_9, slot_10,
                                slot_11, slot_12, slot_13, slot_14, slot_15, slot_16, slot_17, slot_18, slot_19, slot_20,
                                slot_21, slot_22, slot_23, slot_24, slot_25, slot_26, slot_27, slot_28, slot_29, slot_30) 
                                VALUES (@accId, @name, @current, 
                                @s1, @s2, @s3, @s4, @s5, @s6, @s7, @s8, @s9, @s10,
                                @s11, @s12, @s13, @s14, @s15, @s16, @s17, @s18, @s19, @s20,
                                @s21, @s22, @s23, @s24, @s25, @s26, @s27, @s28, @s29, @s30)";

                            using (MySqlCommand insCmd = new MySqlCommand(insertQuery, conn))
                            {
                                insCmd.Parameters.AddWithValue("@accId", spellBook.SummonerId);
                                insCmd.Parameters.AddWithValue("@name", page.Name);
                                insCmd.Parameters.AddWithValue("@current", page.Current ? 1 : 0);

                                for (int i = 1; i <= 30; i++)
                                {
                                    insCmd.Parameters.AddWithValue("@s" + i, slots[i]);
                                }

                                insCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    Console.WriteLine("[SUCCESS] Paginile de Rune (si itemele) salvate pt: " + client._session.Summary.Username);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Eroare DB SaveSpellBook: " + ex.Message);
                }
            }

            e.ReturnRequired = true;
            e.Data = spellBook;
            return e;
        }
    }
}