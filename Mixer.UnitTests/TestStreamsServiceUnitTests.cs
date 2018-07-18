using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.TestStreams;
using Mixer.Base.Model.User;

namespace Mixer.UnitTests
{
    [TestClass]
    public class TestStreamsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetTestStreamSettings()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                PrivatePopulatedUserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > 0);

                TestStreamSettingsModel testStreamSettings = await connection.TestStreams.GetSettings(user.channel);

                Assert.IsNotNull(testStreamSettings);
                Assert.IsTrue(testStreamSettings.id > 0);
            });
        }
    }
}
