using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.MixPlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class MixPlayClientUnitTests : UnitTestBase
    {
        private const string GroupID = "MixerUnitTestGroup";
        private const string SceneID = "MixerUnitTestScene";
        private const string ButtonControlID = "MixerUnitTestButtonControl";
        private const string JoystickControlID = "MixerUnitTestJoystickControl";

        private static MixPlayGameListingModel gameListing;
        private static MixPlayClient client;

        public static MixPlayButtonControlModel CreateTestButton()
        {
            return new MixPlayButtonControlModel()
            {
                controlID = ButtonControlID,
                text = "I'm a button",
                cost = 0,
                disabled = false,
                position = new MixPlayControlPositionModel[]
                {
                    new MixPlayControlPositionModel()
                    {
                        size = "large",
                        width = 10,
                        height = 9,
                        x = 0,
                        y = 0
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "medium",
                        width = 10,
                        height = 3,
                        x = 0,
                        y = 0
                    },
                    new MixPlayControlPositionModel()
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

        public static MixPlayJoystickControlModel CreateTestJoystick()
        {
            return new MixPlayJoystickControlModel()
            {
                controlID = JoystickControlID,
                disabled = false,
                sampleRate = 50,
                position = new MixPlayControlPositionModel[]
                {
                    new MixPlayControlPositionModel()
                    {
                        size = "large",
                        width = 10,
                        height = 9,
                        x = 15,
                        y = 0
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "medium",
                        width = 10,
                        height = 3,
                        x = 15,
                        y = 0
                    },
                    new MixPlayControlPositionModel()
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

                gameListing = await MixPlayServiceUnitTests.CreateTestGame(connection, channel);

                client = await MixPlayClient.CreateFromChannel(connection, channel, gameListing);

                Assert.IsTrue(await client.Connect());
                Assert.IsTrue(await client.Ready());
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await client.Disconnect();

                await MixPlayServiceUnitTests.DeleteTestGame(connection, gameListing);
            });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearPackets();
        }

        [TestMethod]
        public void GetTime()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                DateTimeOffset? dateTime = await client.GetTime();

                Assert.IsNotNull(dateTime);
                Assert.IsTrue(DateTimeOffset.Now.Year.Equals(dateTime.GetValueOrDefault().Year));
            });
        }

        [TestMethod]
        public void GetMemoryStates()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                MixPlayIssueMemoryWarningModel memoryWarning = await client.GetMemoryStates();

                Assert.IsNotNull(memoryWarning);
                Assert.IsNotNull(memoryWarning.resources);
            });
        }

        [TestMethod]
        public void SetBandwidthThrottleAndGetThrottleState()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                MixPlaySetBandwidthThrottleModel bandwidthThrottle = new MixPlaySetBandwidthThrottleModel();
                bandwidthThrottle.AddThrottle("giveInput", 10000000, 3000000);

                bool result = await client.SetBandwidthThrottleWithResponse(bandwidthThrottle);

                Assert.IsTrue(result);

                this.ClearPackets();

                MixPlayGetThrottleStateModel throttleState = await client.GetThrottleState();

                Assert.IsNotNull(throttleState);
                Assert.IsTrue(throttleState.MethodThrottles.Count > 0);
            });
        }

        [TestMethod]
        public void GetAllParticipants()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                MixPlayParticipantCollectionModel participants = await client.GetAllParticipants();

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void GetActiveParticipants()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                DateTimeOffset dateTime = DateTimeOffset.Now;
                MixPlayParticipantCollectionModel participants = await client.GetActiveParticipants(dateTime);

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void UpdateParticipants()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                MixPlayParticipantCollectionModel participants = await client.GetAllParticipants();

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);

                this.ClearPackets();

                participants = await client.UpdateParticipantsWithResponse(participants.participants);

                Assert.IsNotNull(participants);
                Assert.IsNotNull(participants.participants);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteGroup()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                MixPlayConnectedSceneModel testScene = await this.CreateScene(client);

                this.ClearPackets();

                MixPlayGroupModel testGroup = new MixPlayGroupModel()
                {
                    groupID = GroupID,
                    sceneID = testScene.sceneID
                };

                bool result = await client.CreateGroupsWithResponse(new List<MixPlayGroupModel>() { testGroup });

                Assert.IsTrue(result);

                this.ClearPackets();

                MixPlayGroupCollectionModel groups = await client.GetGroups();

                Assert.IsNotNull(groups);
                Assert.IsNotNull(groups.groups);
                Assert.IsTrue(groups.groups.Count > 0);

                testGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals(GroupID));
                MixPlayGroupModel defaultGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals("default"));

                this.ClearPackets();

                groups = await client.UpdateGroupsWithResponse(new List<MixPlayGroupModel>() { testGroup });

                Assert.IsNotNull(groups);
                Assert.IsNotNull(groups.groups);
                Assert.IsTrue(groups.groups.Count > 0);

                testGroup = groups.groups.FirstOrDefault(g => g.groupID.Equals(GroupID));

                this.ClearPackets();

                result = await client.DeleteGroupWithResponse(testGroup, defaultGroup);

                Assert.IsTrue(result);

                await this.DeleteScene(client, testScene);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteScene()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                MixPlayConnectedSceneModel testScene = await this.CreateScene(client);

                this.ClearPackets();

                MixPlayConnectedSceneCollectionModel scenes = await client.UpdateScenesWithResponse(new List<MixPlayConnectedSceneModel>() { testScene });

                Assert.IsNotNull(scenes);
                Assert.IsNotNull(scenes.scenes);
                Assert.IsTrue(scenes.scenes.Count >= 1);

                testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));

                await this.DeleteScene(client, testScene);
            });
        }

        [TestMethod]
        public void CreateUpdateDeleteControl()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                MixPlayConnectedSceneModel testScene = await this.CreateScene(client);

                this.ClearPackets();

                MixPlayControlModel testControl = MixPlayClientUnitTests.CreateTestButton();

                List<MixPlayControlModel> controls = new List<MixPlayControlModel>() { testControl, MixPlayClientUnitTests.CreateTestJoystick() };
                bool result = await client.CreateControlsWithResponse(testScene, controls);

                Assert.IsTrue(result);

                testScene = await this.GetScene(client);
                testControl = testScene.buttons.FirstOrDefault(c => c.controlID.Equals(ButtonControlID));
                Assert.IsNotNull(testControl);

                controls = new List<MixPlayControlModel>() { testControl };
                MixPlayConnectedControlCollectionModel controlCollection = await client.UpdateControlsWithResponse(testScene, controls);

                Assert.IsNotNull(controlCollection);
                Assert.IsNotNull(controlCollection.buttons);

                testScene = await this.GetScene(client);
                testControl = testScene.buttons.FirstOrDefault(c => c.controlID.Equals(ButtonControlID));
                Assert.IsNotNull(testControl);

                result = await client.DeleteControlsWithResponse(testScene, controls);

                Assert.IsTrue(result);

                await this.DeleteScene(client, testScene);
            });
        }

        /// <summary>
        /// Not an effective unit test, as it requires a transaction to actually be sent for this to work
        /// </summary>
        [TestMethod]
        public void CaptureSparkTransaction()
        {
            this.MixPlayWrapper(async (MixerConnection connection, MixPlayClient client) =>
            {
                this.ClearPackets();

                bool result = await client.CaptureSparkTransactionWithResponse(Guid.Empty.ToString());

                Assert.IsTrue(result);
            });
        }

        private void MixPlayWrapper(Func<MixerConnection, MixPlayClient, Task> function)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                client.OnReplyOccurred += MixPlayClient_OnReplyOccurred;
                client.OnMethodOccurred += MixPlayClient_OnMethodOccurred;

                await function(connection, client);

                client.OnReplyOccurred -= MixPlayClient_OnReplyOccurred;
                client.OnMethodOccurred -= MixPlayClient_OnMethodOccurred;
            });
        }

        private async Task<MixPlayConnectedSceneModel> CreateScene(MixPlayClient client)
        {
            this.ClearPackets();

            MixPlayConnectedSceneCollectionModel scenes = await client.CreateScenesWithResponse(new List<MixPlayConnectedSceneModel>() { new MixPlayConnectedSceneModel() { sceneID = SceneID } });

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 1);

            MixPlayConnectedSceneModel testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));
            Assert.IsNotNull(testScene);

            return await this.GetScene(client);
        }

        private async Task<MixPlayConnectedSceneModel> GetScene(MixPlayClient client)
        {
            this.ClearPackets();

            MixPlayConnectedSceneGroupCollectionModel scenes = await client.GetScenes();

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 2);

            MixPlayConnectedSceneModel testScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals(SceneID));
            Assert.IsNotNull(testScene);

            return testScene;
        }

        private async Task DeleteScene(MixPlayClient client, MixPlayConnectedSceneModel scene)
        {
            this.ClearPackets();

            MixPlayConnectedSceneGroupCollectionModel scenes = await client.GetScenes();

            Assert.IsNotNull(scenes);
            Assert.IsNotNull(scenes.scenes);
            Assert.IsTrue(scenes.scenes.Count >= 2);

            MixPlayConnectedSceneModel backupScene = scenes.scenes.FirstOrDefault(s => s.sceneID.Equals("default"));

            bool result = await client.DeleteSceneWithResponse(scene, backupScene);

            Assert.IsTrue(result);
        }

        private void MixPlayClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void MixPlayClient_OnMethodOccurred(object sender, MethodPacket e)
        {
            this.methodPackets.Add(e);
        }
    }
}
