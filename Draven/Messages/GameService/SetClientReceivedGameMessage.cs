using System;
using Draven.ServerModels;
using RtmpSharp.Messaging;

namespace Draven.Messages.GameService
{
    public class SetClientReceivedGameMessage : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            SummonerClient client = sender as SummonerClient;
            Console.WriteLine($"[LOG] Clientul {client._session.Summary.Username} a incarcat ecranul de Champion Select!");

            // Clientul vrea doar o confirmare goala (null) sau un status "true" inapoi
            e.ReturnRequired = true;
            e.Data = null;

            return e;
        }
    }
}