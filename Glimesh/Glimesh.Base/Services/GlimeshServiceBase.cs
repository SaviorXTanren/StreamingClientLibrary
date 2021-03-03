using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using System;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// Base class for all Glimesh services.
    /// </summary>
    public abstract class GlimeshServiceBase : OAuthRestServiceBase
    {
        private const string GlimeshRestAPIBaseAddressFormat = "https://glimesh.tv/api";

        private GlimeshConnection connection;
        private string baseAddress;

        /// <summary>
        /// Creates an instance of the GlimeshServiceBase.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        public GlimeshServiceBase(GlimeshConnection connection) : this(connection, GlimeshRestAPIBaseAddressFormat) { }

        /// <summary>
        /// Creates an instance of the GlimeshServiceBase.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        /// <param name="baseAddress">The base address to use</param>
        public GlimeshServiceBase(GlimeshConnection connection, string baseAddress)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
            this.baseAddress = baseAddress;
        }

        internal GlimeshServiceBase(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <summary>
        /// Performs a GraphQL query with the specified query text and returns the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the result</typeparam>
        /// <param name="query">The query to perform</param>
        /// <param name="key">The key to get for the data</param>
        /// <returns>The result data</returns>
        protected async Task<T> QueryAsync<T>(string query, string key)
        {
            try
            {
                GraphQLHttpClient client = await this.GetGraphQLClient();
                GraphQLResponse<JObject> response = await client.SendQueryAsync<JObject>(new GraphQLRequest(query));
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    foreach (GraphQLError error in response.Errors)
                    {
                        Logger.Log(LogLevel.Error, $"GraphQL Query Error: {query} - {error.Message}");
                    }
                }

                if (response.Data != null && response.Data.ContainsKey(key))
                {
                    return response.Data[key].ToObject<T>();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the OAuth token for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The OAuth token for the connection</returns>
        protected override async Task<OAuthTokenModel> GetOAuthToken(bool autoRefreshToken = true)
        {
            if (this.connection != null)
            {
                return await this.connection.GetOAuthToken(autoRefreshToken);
            }
            return null;
        }

        /// <summary>
        /// Gets the base address for all RESTful calls for this service.
        /// </summary>
        /// <returns>The base address for all RESTful calls</returns>
        protected override string GetBaseAddress() { return this.baseAddress; }

        private async Task<GraphQLHttpClient> GetGraphQLClient(bool autoRefreshToken = true)
        {
            return new GraphQLHttpClient(new GraphQLHttpClientOptions()
            {
                EndPoint = new Uri(this.baseAddress)
            },
            new NewtonsoftJsonSerializer(), await this.GetHttpClient(autoRefreshToken));
        }
    }
}
