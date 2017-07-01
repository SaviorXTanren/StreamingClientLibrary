using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    public abstract class UnitTestBase
    {
        protected List<MethodPacket> methodPackets = new List<MethodPacket>();
        protected List<ReplyPacket> replyPackets = new List<ReplyPacket>();
        protected List<EventPacket> eventPackets = new List<EventPacket>();

        protected void ClearPackets()
        {
            this.methodPackets.Clear();
            this.replyPackets.Clear();
            this.eventPackets.Clear();
        }

        protected void TestWrapper(Func<MixerConnection, Task> function)
        {
            try
            {
                MixerConnection connection = MixerConnectionUnitTests.GetMixerClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
