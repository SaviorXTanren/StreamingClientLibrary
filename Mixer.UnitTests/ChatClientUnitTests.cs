using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
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
        List<ChatReplyPacket> replyPackets = new List<ChatReplyPacket>();
        List<ChatEventPacket> eventPackets = new List<ChatEventPacket>();
        List<ChatMessageEventModel> messages = new List<ChatMessageEventModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearRepliesAndEvents();
        }

        [TestMethod]
        public void ConnectToChat()
        {
            this.ChatWrapper((MixerClient client, ChatClient chatClient) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void AuthenticateToChat()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);
            });
        }

        [TestMethod]
        public void SendMessage()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);
            });
        }

        [TestMethod]
        public void Whisper()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearRepliesAndEvents();

                string messageText = "Hello World!";
                await chatClient.Whisper(chatClient.User.username, messageText);

                await Task.Delay(1000);

                this.ValidateMessage(chatClient, messageText);

                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.data["target"].ToString().Equals(chatClient.User.username));
            });
        }

        [TestMethod]
        public void StartPollAndVote()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearRepliesAndEvents();

                await chatClient.StartVote("Turkey or Ham?", new List<string>() { "Turkey", "Ham" }, 30);

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue(replyPacket.data.ToString().Equals("Poll started."));

                this.ClearRepliesAndEvents();

                await chatClient.ChooseVote(0);

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue((bool)replyPacket.data);
            });
        }

        [TestMethod]
        public void TimeoutUser()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearRepliesAndEvents();

                UserModel user = await client.Users.GetUser("SXTBot");

                await chatClient.TimeoutUser(user.username, 1);

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue(replyPacket.data.ToString().Contains("has been timed out for"));
            });
        }

        [TestMethod]
        public void PurgeUser()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearRepliesAndEvents();

                UserModel user = await client.Users.GetUser("SXTBot");

                await chatClient.PurgeUser(user.username);

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue(string.IsNullOrEmpty(replyPacket.error));
            });
        }

        [TestMethod]
        public void DeleteMessage()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.SendBasicMessage(chatClient);

                ChatMessageEventModel message = this.messages.First();

                this.ClearRepliesAndEvents();

                await chatClient.DeleteMessage(message.id);

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue(replyPacket.data.ToString().Equals("Message deleted."));
            });
        }

        [TestMethod]
        public void ClearMessages()
        {
            this.ChatWrapper(async (MixerClient client, ChatClient chatClient) =>
            {
                await this.AuthenticateChat(chatClient);

                this.ClearRepliesAndEvents();

                await chatClient.ClearMessages();

                await Task.Delay(1000);

                Assert.IsTrue(this.replyPackets.Count > 0);
                ChatReplyPacket replyPacket = this.replyPackets.First();
                Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
                Assert.IsTrue(replyPacket.data.ToString().Equals("Messages cleared."));
            });
        }

        private async Task AuthenticateChat(ChatClient chatClient)
        {
            this.ClearRepliesAndEvents();

            Assert.IsTrue(await chatClient.Authenticate());
        }

        private async Task SendBasicMessage(ChatClient chatClient)
        {
            await this.AuthenticateChat(chatClient);

            this.ClearRepliesAndEvents();

            string messageText = "Hello World!";
            await chatClient.SendMessage(messageText);

            await Task.Delay(1500);

            this.ValidateMessage(chatClient, messageText);
        }

        private void ValidateMessage(ChatClient chatClient, string messageText)
        {
            Assert.IsTrue(this.replyPackets.Count > 0);
            ChatReplyPacket replyPacket = this.replyPackets.First();
            Assert.IsTrue(replyPacket.id == (uint)(chatClient.CurrentPacketID - 1));
            Assert.IsTrue(replyPacket.data["user_name"].ToString().Equals(chatClient.User.username));
            Assert.IsTrue(replyPacket.data["message"]["message"][0]["text"].ToString().Equals(messageText));

            Assert.IsTrue(this.messages.Count > 0);
            ChatMessageEventModel message = this.messages.First();
            Assert.IsTrue(message.message.message.First().text.Equals(messageText));
        }

        private void ChatWrapper(Func<MixerClient, ChatClient, Task> function)
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);
                ChatClient chatClient = await ChatClient.CreateFromChannel(client, channel);

                chatClient.ReplyOccurred += ChatClient_ReplyOccurred;
                chatClient.EventOccurred += ChatClient_EventOccurred;
                chatClient.DisconnectOccurred += ChatClient_DisconnectOccurred;

                chatClient.MessageOccurred += ChatClient_MessageOccurred;

                Assert.IsTrue(await chatClient.Connect());

                await function(client, chatClient);

                await chatClient.Disconnect();
            });
        }

        private void ClearRepliesAndEvents()
        {
            this.replyPackets.Clear();
            this.eventPackets.Clear();
            this.messages.Clear();
        }

        private void ChatClient_ReplyOccurred(object sender, ChatReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void ChatClient_EventOccurred(object sender, ChatEventPacket e)
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
