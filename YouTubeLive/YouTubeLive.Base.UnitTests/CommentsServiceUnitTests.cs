using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeLive.Base.UnitTests
{
    [TestClass]
    public class CommentsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCommentThreadsForChannel()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UCAuUUnT6oDeKwE6v1NGQxug");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<CommentThread> results = await connection.Comments.GetCommentThreadsForChannel(channel, maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetCommentThreadsRelatedToChannel()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UCAuUUnT6oDeKwE6v1NGQxug");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<CommentThread> results = await connection.Comments.GetCommentThreadsRelatedToChannel(channel, maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetCommentThreadsForVideo()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Video video = await connection.Videos.GetVideoByID("_VB39Jo8mAQ");

                Assert.IsNotNull(video);
                Assert.IsNotNull(video.Id);

                IEnumerable<CommentThread> results = await connection.Comments.GetCommentThreadsForVideo(video, maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetCommentsForCommentThread()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                IEnumerable<CommentThread> commentThreads = await connection.Comments.GetCommentThreads(id: "UgzDE2tasfmrYLyNkGt4AaABAg");

                Assert.IsNotNull(commentThreads);
                Assert.IsTrue(commentThreads.Count() > 0);

                IEnumerable<Comment> results = await connection.Comments.GetCommentsForCommentThread(commentThreads.First(), maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
