using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Broadcast;
using Mixer.Base.Model.Clips;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ClipsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetClipsForChannel()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                PrivatePopulatedUserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);

                IEnumerable<ClipModel> clips = await connection.Clips.GetChannelClips(user.channel);

                Assert.IsNotNull(clips);
                Assert.IsTrue(clips.Count() > 0);

                ClipModel clip = await connection.Clips.GetClip(clips.FirstOrDefault().shareableId);

                Assert.IsNotNull(clip);
                Assert.IsTrue(clip.shareableId.Equals(clips.FirstOrDefault().shareableId));
            });
        }

        [TestMethod]
        public void CreateClip()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                BroadcastModel broadcast = await connection.Broadcasts.GetCurrentBroadcast();

                Assert.IsNotNull(broadcast);
                Assert.IsTrue(broadcast.id != Guid.Empty);

                bool canClipBeMade = await connection.Clips.CanClipBeMade(broadcast);

                Assert.IsTrue(canClipBeMade);

                ClipRequestModel clipRequest = new ClipRequestModel()
                {
                    broadcastId = broadcast.id.ToString(),
                    highlightTitle = "Test Clip " + DateTimeOffset.Now.ToString(),
                    clipDurationInSeconds = 30
                };

                ClipModel createdClip = await connection.Clips.CreateClip(clipRequest);

                Assert.IsNotNull(createdClip);
            });
        }
    }
}
