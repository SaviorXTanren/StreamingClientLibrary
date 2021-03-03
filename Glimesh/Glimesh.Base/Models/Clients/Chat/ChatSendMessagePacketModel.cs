﻿using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for sending chat messages.
    /// </summary>
    public class ChatSendMessagePacketModel : ClientPacketModelBase
    {
        private const string MessageMutationPayload = "mutation {{ createChatMessage(channelId: {0}, message: {{ message: \"{1}\" }}) {{ message }} }}";

        /// <summary>
        /// Creates a new instance of the ChatConnectPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="message">The plain-text message to send</param>
        public ChatSendMessagePacketModel(string channelID, string message)
            : base()
        {
            message = message.Replace("\"", "\\\"");

            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(MessageMutationPayload, channelID, message);
            this.Payload["variables"] = new JObject();
        }
    }
}