using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.EventSub;
using Twitch.Base.Services.NewAPI;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class EventSubServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetAllSubscriptions()
        {
            TestAppWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<EventSubSubscriptionModel> results = await connection.NewAPI.EventSub.GetSubscriptions();

                Assert.IsNotNull(results);

                // Clean up previous fails
                foreach (var sub in results)
                {
                    await connection.NewAPI.EventSub.DeleteSubscription(sub.id);
                }

                Assert.IsTrue(results.Count() == 0);
            });
        }


        [TestMethod]
        public void CreateAndDeleteSubscription()
        {
            TestAppWrapper(async (TwitchConnection connection) =>
            {
                var secret = Guid.NewGuid().ToString();

                IEnumerable<EventSubSubscriptionModel> results = await connection.NewAPI.EventSub.GetSubscriptions();
                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 0);

                EventSubSubscriptionModel newSub = await connection.NewAPI.EventSub.CreateSubscription(
                    "channel.follow",
                    "webhook",
                    new Dictionary<string, string> { { "broadcaster_user_id", "12826" } },
                    secret,
                    "https://example.com/webhooks/callback");
                Assert.IsNotNull(newSub);
                Assert.AreEqual("webhook_callback_verification_pending", newSub.status);

                await connection.NewAPI.EventSub.DeleteSubscription(newSub.id);
            });
        }

        [TestMethod]
        public void VerifySignature()
        {
            string body = @"{""subscription"":{""id"":""f1c2a387-161a-49f9-a165-0f21d7a4e1c4"",""type"":""channel.update"",""version"":""1"",""status"":""enabled"",""cost"":0,""condition"":{""broadcaster_user_id"":""1337""},""transport"":{""method"":""webhook"",""callback"":""https://example.com/webhooks/callback""},""created_at"":""2019-11-16T10:11:12.123Z""},""event"":{""broadcaster_user_id"":""1337"",""broadcaster_user_login"":""cool_user"",""broadcaster_user_name"":""Cool_User"",""title"":""Best Stream Ever"",""language"":""en"",""category_id"":""21779"",""category_name"":""Fortnite"",""is_mature"":false}}";
            Assert.IsTrue(EventSubService.VerifySignature("e76c6bd4-55c9-4987-8304-da1588d8988b", "2019-11-16T10:11:12.123Z", body, "sha256=144c8a2a7c859a77bb464e676571f7ffb634f4e34ac64a8794114596ee3eaeea", "secret"));
            Assert.IsFalse(EventSubService.VerifySignature("e76c6bd4-55c9-4987-8304-da1588d8988b", "2019-11-16T10:11:12.123Z", body, "sha256=144c8a2a7c859a77bb464e676571f7ffb634f4e34ac64a8794114596ee3eaeea", "bad-secret"));
        }
    }
}
