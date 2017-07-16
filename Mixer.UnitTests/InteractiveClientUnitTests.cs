using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveClientUnitTests : UnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearPackets();
        }

        [TestMethod]
        public void ConnectToInteractive()
        {
            this.InteractiveWrapper((MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void ReadyInteractive()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);
            });
        }

        [TestMethod]
        public void GetTime()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                DateTimeOffset? dateTime = await interactiveClient.GetTime();

                Assert.IsNotNull(dateTime);
                Assert.IsTrue(DateTimeOffset.UtcNow.Date.Equals(dateTime.GetValueOrDefault().Date));
            });
        }

        [TestMethod]
        public void GetMemoryStates()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveIssueMemoryWarningModel memoryWarning = await interactiveClient.GetMemoryStates();

                Assert.IsNotNull(memoryWarning);
                Assert.IsTrue(memoryWarning.usedBytes > 0);
                Assert.IsTrue(memoryWarning.totalBytes > 0);
                Assert.IsNotNull(memoryWarning.resources);
            });
        }

        [TestMethod]
        public void GetScenes()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveGetScenesModel scenes = await interactiveClient.GetScenes();

                Assert.IsNotNull(scenes);
                Assert.IsTrue(scenes.scenes.Count > 0);
            });
        }

        [TestMethod]
        public void GetAllParticipants()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveGetAllParticipantsModel participants = await interactiveClient.GetAllParticipants();

                Assert.IsNotNull(participants);
                Assert.IsTrue(participants.participants != null);
            });
        }

        [TestMethod]
        public void UpdateControls()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveGetScenesModel scenes = await interactiveClient.GetScenes();

                Assert.IsNotNull(scenes);
                Assert.IsTrue(scenes.scenes.Count > 0);

                InteractiveUpdateControlsModel updateControls = await interactiveClient.UpdateControls(scenes.scenes[0].sceneID, scenes.scenes[0].controls);

                Assert.IsNotNull(updateControls);
                Assert.IsTrue(updateControls.controls.Count() > 0);
            });
        }

        private void InteractiveWrapper(Func<MixerConnection, InteractiveClient, Task> function)
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearPackets();

                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                InteractiveClient interactiveClient = await InteractiveClient.CreateFromChannel(connection, channel, games.First());

                interactiveClient.OnReplyOccurred += InteractiveClient_OnReplyOccurred;
                interactiveClient.OnMethodOccurred += InteractiveClient_OnMethodOccurred;

                Assert.IsTrue(await interactiveClient.Connect());

                await function(connection, interactiveClient);

                await interactiveClient.Disconnect();
            });
        }

        private async Task ReadyInteractive(InteractiveClient interactiveClient)
        {
            this.ClearPackets();

            Assert.IsTrue(await interactiveClient.Ready());
        }

        private void InteractiveClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void InteractiveClient_OnMethodOccurred(object sender, MethodPacket e)
        {
            this.methodPackets.Add(e);
        }
    }
}
