using StreamingClient.Base.ViewModel.Abstract;
using Twitch.Base.Models.NewAPI.Games;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Twitch
{
    public class AbstractTwitchGameViewModel : AbstractGameViewModel
    {
        /// <summary>
        /// The underlying Twitch game.
        /// </summary>
        public GameModel Game { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractTwitchGameViewModel class.
        /// </summary>
        /// <param name="game">The underlying Twitch game</param>
        public AbstractTwitchGameViewModel(GameModel game)
        {
            this.Game = game;
        }

        /// <summary>
        /// The platform of the game.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Twitch; } }
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public override string ID { get { return this.Game.id.ToString(); } }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public override string Name { get { return this.Game.name; } }
    }
}
