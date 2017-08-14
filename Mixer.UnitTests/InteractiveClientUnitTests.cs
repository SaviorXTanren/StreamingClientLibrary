using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveClientUnitTests : UnitTestBase
    {
        private const string GroupID = "MixerUnitTestGroup";
        private const string SceneID = "MixerUnitTestScene";
        private const string ButtonControlID = "MixerUnitTestButtonControl";
        private const string JoystickControlID = "MixerUnitTestJoystickControl";

        private static InteractiveGameListingModel testGameListing;

        public static InteractiveButtonControlModel CreateTestButton()
        {
            return new InteractiveButtonControlModel()
            {
                controlID = ButtonControlID,
                text = "I'm a button",
                cost = 0,
                disabled = false,
                position = new InteractiveControlPositionModel[]
                {
                    new InteractiveControlPositionModel()
                    {
                        size = "large",
                        width = 10,
                        height = 9,
                        x = 0,
                        y = 0
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "medium",
                        width = 10,
                        height = 3,
                        x = 0,
                        y = 0
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "small",
                        width = 10,
                        height = 3,
                        x = 0,
                        y = 0
                    }
                }
            };
        }

        public static InteractiveJoystickControlModel CreateTestJoystick()
        {
            return new InteractiveJoystickControlModel()
            {
                controlID = JoystickControlID,
                disabled = false,
                sampleRate = 50,
                position = new InteractiveControlPositionModel[]
                {
                    new InteractiveControlPositionModel()
                    {
                        size = "large",
                        width = 10,
                        height = 9,
                        x = 15,
                        y = 0
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "medium",
                        width = 10,
                        height = 3,
                        x = 15,
                        y = 0
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "small",
                        width = 10,
                        height = 3,
                        x = 15,
                        y = 0
                    }
                }
            };
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                testGameListing = await InteractiveServiceUnitTests.CreateTestGame(connection, channel);
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await InteractiveServiceUnitTests.DeleteTestGame(connection, testGameListing);
            });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearPackets();
        }

        [TestMethod]
        public void ConnectToInteractive()
        {
            this.InteractiveWrapper((MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void ReadyInteractive()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);
            });
        }

        [TestMethod]
        public void GetTime()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                DateTimeOffset? dateTime = await interactiveClient.GetTime();

                Assert.IsNotNull(dateTime);
                Assert.IsTrue(DateTimeOffset.UtcNow.Date.Equals(dateTime.GetValueOrDefault().Date));
            });
        }

        [TestMethod]
        public void GetMemoryStates()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveIssueMemoryWarningModel memoryWarning = await interactiveClient.GetMemoryStates();

                Assert.IsNotNull(memoryWarning);
                Assert.IsTrue(memoryWarning.totalBytes > 0);
                Assert.IsNotNull(memoryWarning.resources);
            });
        }

        [TestMethod]
        public void SetBandwidthThrottleAndGetThrottleState()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveSetBandwidthThrottleModel bandwidthThrottle = new InteractiveSetBandwidthThrottleModel();
                bandwidthThrottle.AddThrottle("giveInput", 10000000, 3000000);

                bool result = await interactiveClient.SetBandwidthThrottle(bandwidthThrottle);

                Assert.IsTrue(result);

                this.ClearPackets();

                InteractiveGetThrottleStateModel throttleState = await interactiveClient.GetThrottleState();

                Assert.IsNotNull(throttleState);
                Assert.IsTrue(throttleState.MethodThrottles.Count > 0);
            });
        }

        [TestMethod]
        public void GetAllParticipants()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveParticipantCollectionModel participants = await interactiveClient.GetAllParticipants();

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void GetActiveParticipants()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                DateTimeOffset dateTime = DateTimeOffset.Now;
                InteractiveParticipantCollectionModel participants = await interactiveClient.GetActiveParticipants(dateTime);

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void UpdateParticipants()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                InteractiveParticipantCollectionModel participants = await interactiveClient.GetAllParticipants();

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);

                this.ClearPackets();

                participants = await interactiveClient.UpdateParticipants(participants.participants);

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteGroup()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                InteractiveSceneModel testScene = await this.CreateScene(interactiveClient);

                this.ClearPackets();

                InteractiveGroupModel testGroup = new InteractiveGroupModel()
                {
                    groupID = GroupID,
                    sceneID = testScene.sceneID
                };

                bool result = await interactiveClient.CreateGroups(new List<InteractiveGroupModel>() { testGroup });

                Assert.IsTrue(result);

                this.ClearPackets();

                InteractiveGroupCollectionModel groups = await interactiveClient.GetGroups();

                Assert.IsNotNull(groups);
                Assert.IsNotNull(groups.groups);
                Assert.IsTrue(groups.groups.Count > 0);

                testGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals(GroupID));
                InteractiveGroupModel defaultGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals("default"));

                this.ClearPackets();

                groups = await interactiveClient.UpdateGroups(new List<InteractiveGroupModel>() { testGroup });

                Assert.IsNotNull(groups);
                Assert.IsNotNull(groups.groups);
                Assert.IsTrue(groups.groups.Count > 0);

                testGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals(GroupID));

                this.ClearPackets();

                result = await interactiveClient.DeleteGroup(testGroup, defaultGroup);

                Assert.IsTrue(result);

                await this.DeleteScene(interactiveClient, testScene);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteScene()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                InteractiveSceneModel testScene = await this.CreateScene(interactiveClient);

                this.ClearPackets();

                InteractiveSceneCollectionModel scenes = await interactiveClient.UpdateScenes(new List<InteractiveSceneModel>() { testScene });

                Assert.IsNotNull(scenes);
                Assert.IsNotNull(scenes.scenes);
                Assert.IsTrue(scenes.scenes.Count >= 1);

                testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));

                await this.DeleteScene(interactiveClient, testScene);
            });
        }

        [TestMethod]
        public void CreateUpdateDeleteControl()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                InteractiveSceneModel testScene = await this.CreateScene(interactiveClient);

                this.ClearPackets();

                InteractiveControlModel testControl = InteractiveClientUnitTests.CreateTestButton();

                List<InteractiveControlModel> controls = new List<InteractiveControlModel>() { testControl, InteractiveClientUnitTests.CreateTestJoystick() };
                bool result = await interactiveClient.CreateControls(testScene, controls);

                Assert.IsTrue(result);

                testScene = await this.GetScene(interactiveClient);
                testControl = testScene.buttons.FirstOrDefault(c => c.controlID.Equals(ButtonControlID));
                Assert.IsNotNull(testControl);

                controls = new List<InteractiveControlModel>() { testControl };
                InteractiveControlCollectionModel controlCollection = await interactiveClient.UpdateControls(testScene, controls);

                Assert.IsNotNull(controlCollection);
                Assert.IsNotNull(controlCollection.controls);

                testScene = await this.GetScene(interactiveClient);
                testControl = testScene.buttons.FirstOrDefault(c => c.controlID.Equals(ButtonControlID));
                Assert.IsNotNull(testControl);

                result = await interactiveClient.DeleteControls(testScene, controls);

                Assert.IsTrue(result);

                await this.DeleteScene(interactiveClient, testScene);
            });
        }

        /// <summary>
        /// Not an effective unit test, as it requires a transaction to actually be sent for this to work
        /// </summary>
        [TestMethod]
        public void CaptureSparkTransaction()
        {
            this.InteractiveWrapper(async (MixerConnection connection, InteractiveClient interactiveClient) =>
            {
                await this.ReadyInteractive(interactiveClient);

                this.ClearPackets();

                bool result = await interactiveClient.CaptureSparkTransaction(Guid.Empty.ToString());

                Assert.IsTrue(result);
            });
        }

        private void InteractiveWrapper(Func<MixerConnection, InteractiveClient, Task> function)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                InteractiveClient interactiveClient = await InteractiveClient.CreateFromChannel(connection, channel, testGameListing);

                Assert.IsTrue(await interactiveClient.Connect());

                interactiveClient.OnReplyOccurred += InteractiveClient_OnReplyOccurred;
                interactiveClient.OnMethodOccurred += InteractiveClient_OnMethodOccurred;

                await function(connection, interactiveClient);

                await interactiveClient.Disconnect();
            });
        }

        private async Task ReadyInteractive(InteractiveClient interactiveClient)
        {
            this.ClearPackets();

            Assert.IsTrue(await interactiveClient.Ready());
        }

        private async Task<InteractiveSceneModel> CreateScene(InteractiveClient interactiveClient)
        {
            this.ClearPackets();

            InteractiveSceneCollectionModel scenes = await interactiveClient.CreateScenes(new List<InteractiveSceneModel>() { new InteractiveSceneModel(SceneID) });

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 1);

            InteractiveSceneModel testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));
            Assert.IsNotNull(testScene);

            return await this.GetScene(interactiveClient);
        }

        private async Task<InteractiveSceneModel> GetScene(InteractiveClient interactiveClient)
        {
            this.ClearPackets();

            InteractiveSceneCollectionModel scenes = await interactiveClient.GetScenes();

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 2);

            InteractiveSceneModel testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));
            Assert.IsNotNull(testScene);

            return testScene;
        }

        private async Task DeleteScene(InteractiveClient interactiveClient, InteractiveSceneModel scene)
        {
            this.ClearPackets();

            InteractiveSceneCollectionModel scenes = await interactiveClient.GetScenes();

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 2);

            InteractiveSceneModel backupScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals("default"));

            bool result = await interactiveClient.DeleteScene(scene, backupScene);

            Assert.IsTrue(result);
        }

        private void InteractiveClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void InteractiveClient_OnMethodOccurred(object sender, MethodPacket e)
        {
            this.methodPackets.Add(e);
        }
    }
}
