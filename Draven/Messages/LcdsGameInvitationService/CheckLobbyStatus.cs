using System;
using Draven.ServerModels;
using Draven.Structures.Platform.GameInvite;
using RtmpSharp.Messaging;

namespace Draven.Messages.LcdsGameInvitationService
{
    public class CheckLobbyStatus : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;

            // 1. Creăm obiectul pentru deținătorul lobby-ului (Owner)
            Member lobbyOwner = new Member
            {
                // Presupunem că AccountSummary are aceste proprietăți. 
                // Dacă dă eroare la "SummonerId", poți folosi: SummonerId = Convert.ToDouble(client._accId)
                SummonerId = Convert.ToDouble(client._accId),
                SummonerName = client._session.Summary.Username, // sau client._session.Summary.SummonerName
                HasDelegatedInvitePower = true
            };

            // 2. Simulăm un lobby gol / inactiv pentru început
            LobbyStatus status = new LobbyStatus
            {
                InvitationId = Guid.NewGuid().ToString(),
                Members = new RtmpSharp.IO.AMF3.ArrayCollection(),
                Owner = lobbyOwner // Eroarea CS0029 a fost rezolvată aici!
            };

            Console.WriteLine($"[LOG] Checking Lobby Status for {client._session.Summary.Username}");

            // Setăm datele pentru a fi returnate clientului
            e.ReturnRequired = true;
            e.Data = status;

            return e;
        }
    }
}