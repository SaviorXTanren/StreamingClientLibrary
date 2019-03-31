using Mixer.Base.Model.Client;
using System;

namespace Mixer.Base.Util
{
    /// <summary>
    /// An exception for handling bad reply packets.
    /// </summary>
    public class ReplyPacketException : Exception
    {
        /// <summary>
        /// The code for the reply.
        /// </summary>
        public uint Code { get; set; }
        /// <summary>
        /// The path information of the reply.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a new instance of the ReplyPacketException class with the supplied reply error.
        /// </summary>
        /// <param name="model">The reply error that occurred.</param>
        public ReplyPacketException(ReplyErrorModel model) : base(model.message)
        {
            this.Code = model.code;
            this.Path = model.path;
        }
    }
}