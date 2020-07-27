using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Twitch.Webhooks.Base.Models
{
    /// <summary>
    /// The result of a webhook event sent from Twitch.
    /// </summary>
    /// <typeparam name="T">The type that the result contains</typeparam>
    [DataContract]
    public class WebhookResultModel<T>
    {
        /// <summary>
        /// The hash signature included in the header of the request.
        /// </summary>
        [DataMember]
        public string HeaderSignature { get; set; }
        /// <summary>
        /// The content length in the header of the request.
        /// </summary>
        [DataMember]
        public long HeaderContentLength { get; set; }
        /// <summary>
        /// The hash signature computed from the body contents.
        /// </summary>
        [DataMember]
        public string BodySignature { get; set; }
        /// <summary>
        /// The content length computed from the body contents.
        /// </summary>
        [DataMember]
        public long BodyContentLength { get; set; }
        /// <summary>
        /// The query parameters included with the request.
        /// </summary>
        [DataMember]
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// The data included in the request.
        /// </summary>
        [DataMember]
        public IEnumerable<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Creates a new instance of the WebhookResultModel class.
        /// </summary>
        private WebhookResultModel() { }

        /// <summary>
        /// Creates a new instance of the WebhookResultModel class.
        /// </summary>
        /// <param name="headerSignature">The hash signature included in the header of the request.</param>
        /// <param name="headerContentLength">The content length in the header of the request.</param>
        /// <param name="bodySignature">The hash signature computed from the body contents.</param>
        /// <param name="bodyContentLength">The content length computed from the body contents.</param>
        /// <param name="queryParameters">The query parameters included with the request./param>
        /// <param name="data">The data included in the request.</param>
        public WebhookResultModel(string headerSignature, long headerContentLength, string bodySignature, long bodyContentLength, Dictionary<string, string> queryParameters, IEnumerable<T> data)
        {
            this.HeaderSignature = headerSignature;
            this.HeaderContentLength = headerContentLength;
            this.BodySignature = bodySignature;
            this.BodyContentLength = bodyContentLength;
            this.QueryParameters = queryParameters;
            this.Data = data;
        }

        /// <summary>
        /// Whether header hash signature matches the computed hash signature.
        /// </summary>
        [JsonIgnore]
        public bool IsVerified { get { return string.Equals(this.HeaderSignature, this.BodySignature, StringComparison.OrdinalIgnoreCase); } }
    }
}
