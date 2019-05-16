using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace StreamingClient.Base.Model.OAuth
{
    /// <summary>
    /// A Short Code used for getting a token from an OAuth service.
    /// </summary>
    public class OAuthShortCodeModel
    {
        /// <summary>
        /// The code that the user must enter to approve the authentication.
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// The end-URL identifier to use when opening the authentication webpage.
        /// </summary>
        public string handle { get; set; }
        /// <summary>
        /// The expiration time of the short code in seconds from when it was obtained.
        /// </summary>
        [JsonProperty("expires_in")]
        public int expiresIn { get; set; }

        /// <summary>
        /// The time when the token was obtained.
        /// </summary>
        [DataMember]
        public DateTimeOffset AcquiredDateTime { get; set; }

        /// <summary>
        /// The expiration time of the token.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset ExpirationDateTime { get { return this.AcquiredDateTime.AddSeconds(this.expiresIn); } }
    }
}
