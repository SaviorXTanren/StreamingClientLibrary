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
        public static async Task<InteractiveGameListingModel> CreateInteractive2Game(MixerConnection connection, ChannelModel channel, UserModel user, string gameName, InteractiveSceneModel initialScene)
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

            InteractiveGameVersionModel version = gameListing.versions.First();
            version.controls.scenes.Add(initialScene);
            version.controlVersion = "2.0";
            version = await connection.Interactive.UpdateInteractiveGameVersion(version);

            gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);
            gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));

            return gameListing;
        }

        public static InteractiveSceneModel CreateDefaultScene()
        {
            return new InteractiveSceneModel()
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
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "medium",
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "small",
                    }
                }
            };
        }

        public static InteractiveJoystickControlModel CreateJoystick(string controlID, int sampleRate = 50)
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
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "medium",
                    },
                    new InteractiveControlPositionModel()
                    {
                        size = "small",
                    }
                }
            };
        }
    }
}
