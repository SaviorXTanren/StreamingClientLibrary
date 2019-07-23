using System;
using System.Collections.Generic;

namespace Mixer.Base.Model.Clips
{
    /// <summary>
    /// Information required to find a clip.
    /// </summary>
    public class ClipLocatorModel
    {
        /// <summary>
        /// Type of content source this is (e.g. Download, SmoothStreaming, Thumbnail etc.)
        /// </summary>
        public string locatorType { get; set; }
        /// <summary>
        /// URL for this content source
        /// </summary>
        public string uri { get; set; }
    }

    /// <summary>
    /// Information about a clip.
    /// </summary>
    public class ClipModel
    {
        /// <summary>
        /// The content ID of this clip.
        /// </summary>
        public Guid contentId { get; set; }
        /// <summary>
        /// Locator information for this highlight (including the thumbnail and content)
        /// </summary>
        public List<ClipLocatorModel> contentLocators { get; set; }
        /// <summary>
        /// Duration of this highlight in seconds
        /// </summary>
        public uint durationInSeconds { get; set; }
        /// <summary>
        /// Date, in string format, this content will be deleted
        /// </summary>
        public DateTimeOffset expirationDate { get; set; }
        /// <summary>
        /// Title of the highlight (default is the title of the live broadcast the highlight was created from)
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Channel ID of the owner of this highlight
        /// </summary>
        public uint ownerChannelId { get; set; }
        /// <summary>
        /// Mixer title id of source material (as opposed to xbox title id)
        /// </summary>
        public uint typeId { get; set; }
        /// <summary>
        /// ID to get the clip and share with users
        /// </summary>
        public string shareableId { get; set; }
        /// <summary>
        /// Channel ID of the streamer this highlight was clipped from
        /// </summary>
        public uint streamerChannelId { get; set; }
        /// <summary>
        /// Date time, in string format, at which this highlight completed upload
        /// </summary>
        public DateTimeOffset uploadDate { get; set; }
        /// <summary>
        /// Number of views associated with this highlight
        /// </summary>
        public uint viewCount { get; set; }
        /// <summary>
        /// Maturity of the clip (Family = 1, Teen = 2, EighteenPlus = 3)
        /// </summary>
        public uint contentMaturity { get; set; }
        /// <summary>
        /// Tag for clips
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// Creates a new instance of the ClipModel class.
        /// </summary>
        public ClipModel()
        {
            this.contentLocators = new List<ClipLocatorModel>();
            this.tags = new List<string>();
        }
    }
}
