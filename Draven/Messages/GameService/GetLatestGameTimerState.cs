using System;
using Draven.ServerModels;
using RtmpSharp.Messaging;

namespace Draven.Messages.GameService
{
    public class GetLatestGameTimerState : IMessage
    {
        public RemotingMessageReceivedEventArgs HandleMessage(object sender, RemotingMessageReceivedEventArgs e)
        {
            // Păstrăm un timer simplu de test (ex. 60 de secunde)
            Console.WriteLine("[LOG] Trimitere Game Timer catre client.");

            e.ReturnRequired = true;
            // Clientul Air se așteaptă de multe ori ca acest răspuns să fie "CHAMP_SELECT" în sine, 
            // depinde exact ce versiune internă ai. Dacă e 4.20 standard:
            e.Data = "CHAMP_SELECT";

            return e;
        }
    }
}