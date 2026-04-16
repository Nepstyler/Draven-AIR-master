using System;
using Draven.ServerModels;
using Draven.Structures;
using Draven.Structures.Platform.Summoner;
using Draven.Structures.Platform.Catalog;
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

            // Construim runa cu tot cu pachetul de catalog pentru a evita crash-ul in ActionScript
            Action<int, int> addRune = (id, typeId) => {
                RuneType type = new RuneType();
                if (typeId == 1) { type.Id = 1; type.Name = "Red"; }
                else if (typeId == 2) { type.Id = 2; type.Name = "Yellow"; }
                else if (typeId == 3) { type.Id = 3; type.Name = "Blue"; }
                else if (typeId == 4) { type.Id = 4; type.Name = "Black"; }

                inventory.SummonerRunes.Add(new SummonerRune
                {
                    RuneId = id,
                    Quantity = 9,
                    SummonerId = client._sumId,
                    Purchased = DateTime.Now,
                    PurchaseDate = DateTime.Now,
                    Rune = new Rune
                    {
                        ItemId = id,
                        Tier = 3,
                        RuneType = type
                    }
                });
            };

            // Adăugăm Doar Runele TIER 3 Normale (Nivel 30)
            for (int i = 5245; i <= 5274; i++) addRune(i, 1); // Marks
            for (int i = 5275; i <= 5304; i++) addRune(i, 2); // Seals
            for (int i = 5305; i <= 5334; i++) addRune(i, 3); // Glyphs
            for (int i = 5335; i <= 5374; i++) addRune(i, 4); // Quints

            // Adăugăm Runele Speciale TIER 3 (Halloween, Razer, Intel, etc)
            int[] specialMarks = { 8002, 8007, 8008, 8024, 8025, 10001 };
            foreach (int sm in specialMarks) addRune(sm, 1);

            int[] specialSeals = { 8004, 8006, 8009, 8010, 8026, 8027, 8028 };
            foreach (int ss in specialSeals) addRune(ss, 2);

            int[] specialGlyphs = { 8003, 8005, 8011, 8012, 8029, 8030, 8031 };
            foreach (int sg in specialGlyphs) addRune(sg, 3);

            int[] specialQuints = { 8001, 8013, 8014, 8015, 8016, 8017, 8018, 8019, 8020, 8021, 8022, 8023, 8035, 10002 };
            foreach (int sq in specialQuints) addRune(sq, 4);

            e.ReturnRequired = true;
            e.Data = inventory;
            return e;
        }
    }
}