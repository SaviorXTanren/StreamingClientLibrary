using Mixer.Base.Util;
using System;
using System.Collections.Generic;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Represents a single emoticon in an emoticon pack
    /// </summary>
    public class EmoticonGroupModel
    {
        /// <summary>
        /// The X coordinate of the top left corner of this emoticon.
        /// </summary>
        public uint x { get; set; }

        /// <summary>
        /// The Y coordinate of the top left corner of this emoticon.
        /// </summary>
        public uint y { get; set; }

        /// <summary>
        /// The width of this emoticon.
        /// </summary>
        public uint width { get; set; }

        /// <summary>
        /// The height of this emoticon.
        /// </summary>
        public uint height { get; set; }
    }

    /// <summary>
    /// Represents a channel's entire emoticon pack.
    /// </summary>
    public class EmoticonPackModel
    {
        /// <summary>
        /// The related channel ID.
        /// </summary>
        public uint channelId { get; set; }

        /// <summary>
        /// The URI of the emoticon sprite sheet.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// The list of available emoticons in the pack.  The key is the emoticon shortcut text.
        /// </summary>
        public Dictionary<string, EmoticonGroupModel> emoticons { get; set; }
    }
}
