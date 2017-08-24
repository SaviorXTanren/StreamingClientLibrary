using Newtonsoft.Json;
using System;

namespace Mixer.Base.Model.OAuth
{
    public class OAuthTokenModel
    {
        public string clientID { get; set; }
        public string authorizationCode { get; set; }

        [JsonProperty("refresh_token")]
        public string refreshToken { get; set; }
        [JsonProperty("access_token")]
        public string accessToken { get; set; }
        [JsonProperty("expires_in")]
        public int expiresIn { get; set; }

        [JsonIgnore]
        public DateTimeOffset Expiration { get { return DateTimeOffset.Now.AddSeconds(this.expiresIn); } }
    }
}
