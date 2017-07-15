using Mixer.Base.Model.Client;
using System;

namespace Mixer.Base.Util
{
    public class ReplyPacketException : Exception
    {
        public uint Code { get; set; }
        public string Path { get; set; }

        public ReplyPacketException(uint code, string message, string path = null) : base(message)
        {
            this.Code = code;
            this.Path = path;
        }

        public ReplyPacketException(ReplyErrorModel model) : base(model.message)
        {
            this.Code = model.code;
            this.Path = model.path;
        }
    }
}