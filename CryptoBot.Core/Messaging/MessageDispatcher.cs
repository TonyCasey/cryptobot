using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace CryptoBot.Core.Messaging
{
    public class MessageDispatcher
    {
        
        private static readonly List<IMessageHandler> s_Handlers =
        new List<IMessageHandler>();


        public void Dispatch(IMessage msg)
        {
            foreach (IMessageHandler messageHandler in s_Handlers)
            {
                messageHandler.Process(msg);
            }
        }

        public void AddMessageHandler(IMessageHandler messageHandler)
        {
            if (!s_Handlers.Contains(messageHandler))
            {
                s_Handlers.Add(messageHandler);
            }

        }
    }
    
    public interface IMessageHandler
    {
        void Process(IMessage msg);
    }

}
