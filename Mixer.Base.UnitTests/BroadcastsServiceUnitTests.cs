using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Broadcast;
using System;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class BroadcastsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentBroadcast()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                BroadcastModel broadcast = await connection.Broadcasts.GetCurrentBroadcast();

                Assert.IsNotNull(broadcast);
                Assert.IsTrue(broadcast.id != Guid.Empty);

                broadcast = await connection.Broadcasts.GetBroadcast(broadcast.id);

                Assert.IsNotNull(broadcast);
                Assert.IsTrue(broadcast.id != Guid.Empty);
            });
        }
    }
}
