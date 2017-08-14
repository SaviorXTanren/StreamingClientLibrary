using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Interactive
{
    public static class InteractiveGameHelper
    {
        public static async Task<InteractiveGameListingModel> CreateInteractive2Game(MixerConnection connection, ChannelModel channel, UserModel user, string gameName, InteractiveGameControlsSceneModel initialScene)
        {
            InteractiveGameModel game = new InteractiveGameModel()
            {
                name = gameName,
                ownerId = user.id,
            };
            game = await connection.Interactive.CreateInteractiveGame(game);

            game.controlVersion = "2.0";
            game = await connection.Interactive.UpdateInteractiveGame(game);

            IEnumerable<InteractiveGameListingModel> gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);
            InteractiveGameListingModel gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));

            InteractiveVersionModel version = gameListing.versions.First();
            version.controls.scenes.Add(initialScene);
            version.controlVersion = "2.0";
            version = await connection.Interactive.UpdateInteractiveGameVersion(version);

            gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);
            gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));

            return gameListing;
        }

        public static InteractiveGameControlsSceneModel CreateDefaultScene(IEnumerable<InteractiveControlModel> controls)
        {
            return new InteractiveGameControlsSceneModel()
            {
                sceneID = "default",
            };
        }

        public static InteractiveButtonControlModel CreateButton(string controlID, string buttonText, int cost = 0)
        {
            return new InteractiveButtonControlModel()
            {
                controlID = controlID,
                text = buttonText,
                cost = cost,
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

        public static InteractiveJoystickControlModel CreateJoystick(string controlID, int sampleRate)
        {
            return new InteractiveJoystickControlModel()
            {
                controlID = controlID,
                sampleRate = sampleRate,
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
    }
}
