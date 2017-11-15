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
        private static ChatClient chatClient;

        private List<ChatMessageEventModel> messages = new List<ChatMessageEventModel>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
                chatClient = await ChatClient.CreateFromChannel(connection, channel);

                Assert.IsTrue(await chatClient.Connect());
                Assert.IsTrue(await chatClient.Authenticate());
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await chatClient.Disconnect();
            });
        }

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
        public void Ping()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                bool ping = await chatClient.Ping();
                Assert.IsTrue(ping);
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
                this.ClearAllPackets();

                string messageText = "Hello World!";
                ChatMessageEventModel message = await chatClient.WhisperWithResponse(chatClient.User.username, messageText);

                this.ValidateMessage(chatClient, message, messageText);

                Assert.IsTrue(message.target.ToString().Equals(chatClient.User.username));
            });
        }

        [TestMethod]
        public void StartPollAndVote()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                this.ClearAllPackets();

                bool result = await chatClient.StartVoteWithResponse("Turkey or Ham?", new List<string>() { "Turkey", "Ham" }, 30);
                Assert.IsTrue(result);

                this.ClearAllPackets();

                result = await chatClient.ChooseVoteWithResponse(0);
                Assert.IsTrue(result);
            });
        }

        /// <summary>
        /// Requires user that is being timed out to be logged in to the chat for the channel
        /// </summary>
        [TestMethod]
        public void TimeoutUser()
        {
            this.ChatWrapper(async (MixerConnection connection, ChatClient chatClient) =>
            {
                this.ClearAllPackets();

                UserModel user = await connection.Users.GetUser("SXTBot");

                bool result = await chatClient.TimeoutUserWithResponse(user.username, 60);
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

                bool result = await chatClient.PurgeUserWithResponse(user.username);
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

                bool result = await chatClient.DeleteMessageWithResponse(message.id);
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

                bool result = await chatClient.ClearMessagesWithResponse();
                Assert.IsTrue(result);
            });
        }

        private async Task SendBasicMessage(ChatClient chatClient)
        {
            this.ClearAllPackets();

            string messageText = "Hello World!";
            ChatMessageEventModel message = await chatClient.SendMessageWithResponse(messageText);

            this.ValidateMessage(chatClient, message, messageText);
        }

        private void ValidateMessage(ChatClient chatClient, ChatMessageEventModel message, string messageText)
        {
            Assert.IsNotNull(message);
            Assert.IsTrue(message.user_name.ToString().Equals(chatClient.User.username));
            Assert.IsTrue(message.message.message.First().text.Equals(messageText));
        }

        private void ChatWrapper(Func<MixerConnection, ChatClient, Task> function)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearAllPackets();

                chatClient.OnReplyOccurred += ChatClient_OnReplyOccurred;
                chatClient.OnEventOccurred += ChatClient_OnEventOccurred;
                chatClient.OnDisconnectOccurred += ChatClient_DisconnectOccurred;
                chatClient.OnMessageOccurred += ChatClient_MessageOccurred;

                await function(connection, chatClient);

                chatClient.OnReplyOccurred -= ChatClient_OnReplyOccurred;
                chatClient.OnEventOccurred -= ChatClient_OnEventOccurred;
                chatClient.OnDisconnectOccurred -= ChatClient_DisconnectOccurred;
                chatClient.OnMessageOccurred -= ChatClient_MessageOccurred;
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
