using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Patronage;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class PatronageServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetPatronageStatus()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetChannel("MyBoomShtick");

                Assert.IsNotNull(channel);

                PatronageStatusModel patronageStatus = await connection.Patronage.GetPatronageStatus(channel);

                Assert.IsNotNull(patronageStatus);

                PatronagePeriodModel patronagePeriod = await connection.Patronage.GetPatronagePeriod(patronageStatus.patronagePeriodId);

                Assert.IsNotNull(patronagePeriod);
            });
        }
    }
}
