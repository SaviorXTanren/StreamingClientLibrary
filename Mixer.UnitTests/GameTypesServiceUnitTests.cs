using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class GameTypesServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetGameTypesAndChannels()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                string gameName = "Maui";
                IEnumerable<GameTypeModel> gameTypes = await connection.GameTypes.GetGameTypes(gameName, 1);

                Assert.IsNotNull(gameTypes);
                Assert.IsTrue(gameTypes.Count() > 0);
                Assert.IsTrue(gameTypes.Any(gt => gt.name.Equals(gameName)));

                IEnumerable<ChannelModel> channels = await connection.GameTypes.GetChannelsByGameType(searchedGameTypes.First(), 1);

                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
            });
        }
    }
}
