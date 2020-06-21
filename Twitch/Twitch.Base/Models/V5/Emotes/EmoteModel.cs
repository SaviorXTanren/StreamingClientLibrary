namespace Twitch.Base.Models.V5.Emotes
{
    /// <summary>
    /// Information about a user emote.
    /// </summary>
    public class EmoteModel
    {
        private const string EmoteURLFormat = "https://static-cdn.jtvnw.net/emoticons/v1/{0}/{1}";

        /// <summary>
        /// The ID of the emote.
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// The ID of the set containing this emote.
        /// </summary>
        public string setID { get; set; }
        /// <summary>
        /// The text code for the emote.
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// The image url of the emote.
        /// </summary>
        public string URL { get { return this.Size1URL; } }

        /// <summary>
        /// The size 1 image url of the emote.
        /// </summary>
        public string Size1URL { get { return string.Format(EmoteModel.EmoteURLFormat, this.id, "1.0"); } }
        /// <summary>
        /// The size 2 image url of the emote.
        /// </summary>
        public string Size2URL { get { return string.Format(EmoteModel.EmoteURLFormat, this.id, "2.0"); } }
        /// <summary>
        /// The size 3 image url of the emote.
        /// </summary>
        public string Size3URL { get { return string.Format(EmoteModel.EmoteURLFormat, this.id, "3.0"); } }
    }
}
