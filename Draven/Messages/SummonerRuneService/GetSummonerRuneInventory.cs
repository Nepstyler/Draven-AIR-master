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

            // ID-urile pentru runele de bază Season 4 (Marks, Seals, Glyphs, Quints)
            int[] runeIds = new int[] { 5245, 5317, 5289, 5335, 5251, 5311, 5253, 5357, 5273 };

            foreach (int id in runeIds)
            {
                SummonerRune rune = new SummonerRune();
                rune.RuneId = id;      // Setează ID-ul magiei
                rune.Quantity = 9;     // Primești 9 bucăți din fiecare pentru a umple sloturile
                rune.SummonerId = client._sumId;

                inventory.SummonerRunes.Add(rune);
            }

            e.ReturnRequired = true;
            e.Data = inventory;

            return e;
        }
    }
}