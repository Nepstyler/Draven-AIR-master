using System;
using Draven.ServerModels;
using Draven.Structures;
using Draven.Structures.Platform.Summoner;
using RtmpSharp.IO.AMF3;
using RtmpSharp.Messaging;

namespace Draven.Messages.SummonerRuneService
{
    class GetSummonerRuneInventory : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            SummonerRuneInventory inventory = new SummonerRuneInventory
            {
                DateString = DateTime.Now.ToString(),
                SummonerId = client._sumId,
                SummonerRunes = new ArrayCollection()
            };

            Action<int> addRune = (id) => {
                inventory.SummonerRunes.Add(new SummonerRune
                {
                    RuneId = id,
                    Quantity = 9, // Acum clientul va citi asta corect si nu le mai face gri!
                    SummonerId = client._sumId,
                    Purchased = DateTime.Now,
                    PurchaseDate = DateTime.Now
                });
            };

            // Injectam TOATE runele din joc (Marks, Seals, Glyphs, Quints normale)
            for (int i = 5000; i <= 5450; i++)
            {
                addRune(i);
            }

            // Injectam TOATE runele speciale de Halloween/Craciun/Intel/Razer
            for (int i = 8000; i <= 8035; i++)
            {
                addRune(i);
            }

            // Cateva in plus de pe Wiki
            addRune(10001); // Razer Mark of Precision
            addRune(10002); // Razer Quintessence of Speed

            e.ReturnRequired = true;
            e.Data = inventory;
            return e;
        }
    }
}