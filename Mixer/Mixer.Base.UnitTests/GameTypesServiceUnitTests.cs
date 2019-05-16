using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class GameTypesServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetGameTypesAndChannels()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<GameTypeModel> gameTypes = await connection.GameTypes.GetGameTypes(100);
                Assert.IsNotNull(gameTypes);
                Assert.IsTrue(gameTypes.Count() >= 100);

                string gameName = "Web Show";
                gameTypes = await connection.GameTypes.GetGameTypes(gameName, 1);

                Assert.IsNotNull(gameTypes);
                Assert.IsTrue(gameTypes.Count() > 0);
                Assert.IsTrue(gameTypes.Any(gt => gt.name.Equals(gameName)));

                GameTypeModel gameType = await connection.GameTypes.GetGameType(gameTypes.First().id);

                Assert.IsNotNull(gameType);
                Assert.IsTrue(gameType.name.Equals(gameName));

                IEnumerable<ChannelModel> channels = await connection.GameTypes.GetChannelsByGameType(gameTypes.First(), 1);

                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
            });
        }
    }
}
