using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Skills;
using Mixer.Base.Model.User;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class SkillsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetPatronageStatus()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);

                ChannelModel channel = await connection.Channels.GetChannel(user.username);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.id > (uint)0);

                Assert.IsNotNull(channel);

                SkillCatalogModel skillCatalog = await connection.Skills.GetSkillCatalog(channel);

                Assert.IsNotNull(skillCatalog);
                Assert.IsTrue(skillCatalog.skills.Length > 0);
            });
        }
    }
}
