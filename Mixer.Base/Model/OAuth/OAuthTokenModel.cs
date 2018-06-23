using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Mixer.Base.Model.OAuth
{
    public class OAuthTokenModel
    {
        public string clientID { get; set; }
        public string clientSecret { get; set; }
        public string authorizationCode { get; set; }

        [JsonProperty("refresh_token")]
        public string refreshToken { get; set; }
        [JsonProperty("access_token")]
        public string accessToken { get; set; }
        [JsonProperty("expires_in")]
        public int expiresIn { get; set; }

        [DataMember]
        public DateTimeOffset AcquiredDateTime { get; set; }
        [JsonIgnore]
        public DateTimeOffset ExpirationDateTime { get { return this.AcquiredDateTime.AddSeconds(this.expiresIn); } }

        public OAuthTokenModel()
        {
            this.AcquiredDateTime = DateTimeOffset.Now;
        }
    }
}
