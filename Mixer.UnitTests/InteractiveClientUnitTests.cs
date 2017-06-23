using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveClientUnitTests : UnitTestBase
    {
        List<InteractiveReplyPacket> replyPackets = new List<InteractiveReplyPacket>();
        List<InteractiveMethodPacket> methodPackets = new List<InteractiveMethodPacket>();

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearRepliesAndEvents();
        }

        [TestMethod]
        public void ConnectToInteractive()
        {
            this.InteractiveWrapper((MixerClient client, InteractiveClient interactiveClient) =>
            {
                return Task.FromResult(0);
            });
        }

        private void InteractiveWrapper(Func<MixerClient, InteractiveClient, Task> function)
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);
                InteractiveClient interactiveClient = await InteractiveClient.CreateFromChannel(client, channel);

                Assert.IsTrue(await interactiveClient.Ready());

                await function(client, interactiveClient);

                await interactiveClient.Disconnect();
            });
        }

        private void ClearRepliesAndEvents()
        {
            this.replyPackets.Clear();
            this.methodPackets.Clear();
        }
    }
}
