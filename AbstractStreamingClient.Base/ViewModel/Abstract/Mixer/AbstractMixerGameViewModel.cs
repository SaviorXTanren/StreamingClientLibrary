using Mixer.Base.Model.Game;
using StreamingClient.Base.ViewModel.Abstract;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Mixer
{
    public class AbstractMixerGameViewModel : AbstractGameViewModel
    {
        /// <summary>
        /// The underlying Mixer game.
        /// </summary>
        public GameTypeSimpleModel Game { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractMixerGameViewModel class.
        /// </summary>
        /// <param name="game">The underlying Mixer game</param>
        public AbstractMixerGameViewModel(GameTypeSimpleModel game)
        {
            this.Game = game;
        }

        /// <summary>
        /// The platform of the game.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Mixer; } }
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
