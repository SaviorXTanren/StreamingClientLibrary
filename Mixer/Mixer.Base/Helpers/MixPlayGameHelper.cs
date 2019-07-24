using Mixer.Base.Model.Channel;
using Mixer.Base.Model.MixPlay;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Interactive
{
    /// <summary>
    /// Helper methods for creating MixPlay games.
    /// </summary>
    public static class MixPlayGameHelper
    {
        /// <summary>
        /// Creates a MixPlay game.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        /// <param name="channel">The owning channel</param>
        /// <param name="user">The owning user</param>
        /// <param name="gameName">The name of the game</param>
        /// <param name="initialScene">The initial scene</param>
        /// <returns>The created MixPlay game</returns>
        public static async Task<MixPlayGameListingModel> CreateMixPlay2Game(MixerConnection connection, ChannelModel channel, UserModel user, string gameName, MixPlaySceneModel initialScene)
        {
            MixPlayGameModel game = new MixPlayGameModel()
            {
                name = gameName,
                ownerId = user.id,
            };
            game = await connection.MixPlay.CreateMixPlayGame(game);

            game.controlVersion = "2.0";
            game = await connection.MixPlay.UpdateMixPlayGame(game);

            IEnumerable<MixPlayGameListingModel> gameListings = await connection.MixPlay.GetOwnedMixPlayGames(channel);
            MixPlayGameListingModel gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));

            MixPlayGameVersionModel version = gameListing.versions.First();
            version.controls.scenes.Add(initialScene);
            version.controlVersion = "2.0";
            version = await connection.MixPlay.UpdateMixPlayGameVersion(version);

            gameListings = await connection.MixPlay.GetOwnedMixPlayGames(channel);
            gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));

            return gameListing;
        }

        /// <summary>
        /// Creates a default scene.
        /// </summary>
        /// <returns>The default scene.</returns>
        public static MixPlaySceneModel CreateDefaultScene()
        {
            return new MixPlaySceneModel()
            {
                sceneID = "default",
            };
        }

        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="controlID">The ID of the control</param>
        /// <param name="buttonText">The text of the button</param>
        /// <param name="cost">The cost of the button</param>
        /// <returns>The created button</returns>
        public static MixPlayButtonControlModel CreateButton(string controlID, string buttonText, int cost = 0)
        {
            return new MixPlayButtonControlModel()
            {
                controlID = controlID,
                text = buttonText,
                cost = cost,
                disabled = false,
                position = new MixPlayControlPositionModel[]
                {
                    new MixPlayControlPositionModel()
                    {
                        size = "large",
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "medium",
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "small",
                    }
                }
            };
        }

        /// <summary>
        /// Creates a joystick.
        /// </summary>
        /// <param name="controlID">The ID of the control</param>
        /// <param name="sampleRate">The sample rate for input</param>
        /// <returns>The created joystick</returns>
        public static MixPlayJoystickControlModel CreateJoystick(string controlID, int sampleRate = 50)
        {
            return new MixPlayJoystickControlModel()
            {
                controlID = controlID,
                sampleRate = sampleRate,
                disabled = false,
                position = new MixPlayControlPositionModel[]
                {
                    new MixPlayControlPositionModel()
                    {
                        size = "large",
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "medium",
                    },
                    new MixPlayControlPositionModel()
                    {
                        size = "small",
                    }
                }
            };
        }
    }
}
