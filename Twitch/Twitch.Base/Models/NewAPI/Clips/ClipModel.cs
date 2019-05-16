namespace Twitch.Base.Models.NewAPI.Clips
{
    /// <summary>
    /// Information about a clip.
    /// </summary>
    public class ClipModel
    {
        /// <summary>
        /// The ID of the clip.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The url of the clip.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The url of the embeddable clip.
        /// </summary>
        public string embed_url { get; set; }
        /// <summary>
        /// The ID of the broadcaster of the clip.
        /// </summary>
        public string broadcaster_id { get; set; }
        /// <summary>
        /// The name of the broadcaster of the clip.
        /// </summary>
        public string broadcaster_name { get; set; }
        /// <summary>
        /// The ID of the creator of the clip.
        /// </summary>
        public string creator_id { get; set; }
        /// <summary>
        /// The name of the creator of the clip.
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// The ID of the video.
        /// </summary>
        public string video_id { get; set; }
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public string game_id { get; set; }
        /// <summary>
        /// The language identifier.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The title of the clip.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The view count of the clip.
        /// </summary>
        public long view_count { get; set; }
        /// <summary>
        /// The date the clip was created at.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The url of the clip thumbnail.
        /// </summary>
        public string thumbnail_url { get; set; }
    }
}
