using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ChatClientUnitTests : UnitTestBase
    {
        List<ChatMessageEventModel> messages = new List<ChatMessageEventModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearAllPackets();
        }

        [TestMethod]
        public void ConnectToChat()
        {
            this.ChatWrapper((MixerConnection connection, ChatClient chatClient) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void AuthenticateToChat()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);
            });
        }

        [TestMethod]
        public void SendMessage()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);
            });
        }

        [TestMethod]
        public void Whisper()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearAllPackets();

                string messageText = "Hello World!";
                ChatMessageEventModel message = await chatClient.Whisper(chatClient.User.username, messageText);

                this.ValidateMessage(chatClient, message, messageText);

                Assert.IsTrue(message.target.ToString().Equals(chatClient.User.username));
            });
        }

        [TestMethod]
        public void StartPollAndVote()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearAllPackets();

                bool result = await chatClient.StartVote("Turkey or Ham?", new List<string>() { "Turkey", "Ham" }, 30);
                Assert.IsTrue(result);

                this.ClearAllPackets();

                result = await chatClient.ChooseVote(0);
                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void TimeoutUser()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);

                this.ClearAllPackets();

                UserModel user = await connection.Users.GetUser("SXTBot");

                bool result = await chatClient.TimeoutUser(user.username, 65000);
                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void PurgeUser()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);

                this.ClearAllPackets();

                UserModel user = await connection.Users.GetUser("SXTBot");

                bool result = await chatClient.PurgeUser(user.username);
                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void DeleteMessage()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);

                ChatMessageEventModel message = this.messages.First();

                this.ClearAllPackets();

                bool result = await chatClient.DeleteMessage(message.id);
                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void ClearMessages()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);

                this.ClearAllPackets();

                bool result = await chatClient.ClearMessages();
                Assert.IsTrue(result);
            });
        }

        private async Task AuthenticateChat(ChatClient chatClient)
        {
            this.ClearAllPackets();

            Assert.IsTrue(await chatClient.Authenticate());
        }

        private async Task SendBasicMessage(ChatClient chatClient)
        {
            await this.AuthenticateChat(chatClient);

            this.ClearAllPackets();

            string messageText = "Hello World!";
            ChatMessageEventModel message = await chatClient.SendMessage(messageText);

            this.ValidateMessage(chatClient, message, messageText);
        }

        private void ValidateMessage(ChatClient chatClient, ChatMessageEventModel message, string messageText)
        {
            Assert.IsTrue(message.user_name.ToString().Equals(chatClient.User.username));
            Assert.IsTrue(message.message.message.First().text.Equals(messageText));
        }

        private void ChatWrapper(Func<MixerConnection, ChatClient, Task> function)
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearAllPackets();

                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
                ChatClient chatClient = await ChatClient.CreateFromChannel(connection, channel);

                chatClient.OnReplyOccurred += ChatClient_OnReplyOccurred;
                chatClient.OnEventOccurred += ChatClient_OnEventOccurred;
                chatClient.OnDisconnectOccurred += ChatClient_DisconnectOccurred;

                chatClient.OnMessageOccurred += ChatClient_MessageOccurred;

                Assert.IsTrue(await chatClient.Connect());

                await function(connection, chatClient);

                await chatClient.Disconnect();
            });
        }

        private void ClearAllPackets()
        {
            this.ClearPackets();
            this.messages.Clear();
        }

        private void ChatClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void ChatClient_OnEventOccurred(object sender, EventPacket e)
        {
            this.eventPackets.Add(e);
        }

        private void ChatClient_DisconnectOccurred(object sender, WebSocketCloseStatus e)
        {
            Assert.Fail("Disconnection occurred: " + e.ToString());
        }

        private void ChatClient_MessageOccurred(object sender, ChatMessageEventModel e)
        {
            this.messages.Add(e);
        }
    }
}
