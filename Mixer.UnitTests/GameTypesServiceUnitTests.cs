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
                IEnumerable<GameTypeModel> gameTypes = await connection.GameTypes.GetGameTypes(1);

                Assert.IsNotNull(gameTypes);
                Assert.IsTrue(gameTypes.Count() > 0);

                IEnumerable<GameTypeSimpleModel> searchedGameTypes = await connection.GameTypes.GetGameTypesByLookup(titleName: "PLAYERUNKNOWN'S BATTLEGROUNDS");

                Assert.IsNotNull(searchedGameTypes);
                Assert.IsTrue(searchedGameTypes.Count() > 0);

                IEnumerable<ChannelModel> channels = await connection.GameTypes.GetChannelsByGameType(searchedGameTypes.First(), 1);

                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
            });
        }
    }
}
