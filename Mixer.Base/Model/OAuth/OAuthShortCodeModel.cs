using System;
using System.Runtime.Serialization;

namespace Mixer.Base.Model.OAuth
{
    public class OAuthShortCodeModel
    {
        public string code { get; set; }
        public string handle { get; set; }
        public uint expires_in { get; set; }

        [DataMember]
        public DateTimeOffset AcquiredDateTime { get; set; }

        public OAuthShortCodeModel()
        {
            this.AcquiredDateTime = DateTimeOffset.Now;
        }
    }
}
