using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for Chat-based services.
    /// </summary>
    public class ChatsService : MixerServiceBase
    {
        /// <summary>
        /// https://dev.mixer.com/content/blog/chatlistdeprecations
        /// </summary>
        private class ChatsV2Service : MixerServiceBase
        {
            /// <summary>
            /// Creates an instance of the ChatsV2Service.
            /// </summary>
            /// <param name="connection">The Mixer connection to use</param>
            public ChatsV2Service(MixerConnection connection) : base(connection, 2) { }

            /// <summary>
            /// Gets the current users connected to chat for the specified channel. The search can be limited to a maximum number
            /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
            /// threshold and slightly more than the maximum number may be returned.
            /// </summary>
            /// <param name="channel">The channel to get chat users for</param>
            /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
            /// <returns>The chat users</returns>
            public async Task<IEnumerable<ChatUserModel>> GetUsers(ChannelModel channel, uint maxResults = 1)
            {
                Validator.ValidateVariable(channel, "channel");
                return await this.GetPagedContinuationAsync<ChatUserModel>("chats/" + channel.id + "/users", maxResults);
            }

            /// <summary>
            /// Gets the current users connected to chat for the specified channel. The search can be limited to a maximum number
            /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
            /// threshold and slightly more than the maximum number may be returned.
            /// </summary>
            /// <param name="channel">The channel to get chat users for</param>
            /// <param name="processResults">The function to process results as they come in</param>
            /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
            /// <returns>The chat users</returns>
            public async Task GetUsers(ChannelModel channel, Func<IEnumerable<ChatUserModel>, Task> processResults, uint maxResults = 1)
            {
                Validator.ValidateVariable(channel, "channel");
                await this.GetPagedContinuationAsync<ChatUserModel>("chats/" + channel.id + "/users", processResults, maxResults);
            }
        }

        private ChatsV2Service chatsV2Service;

        /// <summary>
        /// Creates an instance of the ChatsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public ChatsService(MixerConnection connection)
            : base(connection)
        {
            this.chatsV2Service = new ChatsV2Service(connection);
        }

        /// <summary>
        /// Gets the chat information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get chat information for</param>
        /// <returns>The chat information</returns>
        public async Task<ChannelChatModel> GetChat(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelChatModel>("chats/" + channel.id);
        }

        /// <summary>
        /// Gets the specified user ID's chat information in relation to the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get chat users for</param>
        /// <param name="userID">The ID of the user</param>
        /// <returns>The chat information for the specified user</returns>liter
        public async Task<ChatUserModel> GetUser(ChannelModel channel, uint userID)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChatUserModel>("chats/" + channel.id + "/users/" + userID);
        }

        /// <summary>
        /// Gets the current users connected to chat for the specified channel. The search can be limited to a maximum number
        /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
        /// threshold and slightly more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get chat users for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The chat users</returns>
        public async Task<IEnumerable<ChatUserModel>> GetUsers(ChannelModel channel, uint maxResults = 1)
        {
            return await this.chatsV2Service.GetUsers(channel, maxResults);
        }

        /// <summary>
        /// Gets the current users connected to chat for the specified channel. The search can be limited to a maximum number
        /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
        /// threshold and slightly more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get chat users for</param>
        /// <param name="processResults">The function to process results as they come in</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The chat users</returns>
        public async Task GetUsers(ChannelModel channel, Func<IEnumerable<ChatUserModel>, Task> processResults, uint maxResults = 1)
        {
            await this.chatsV2Service.GetUsers(channel, processResults, maxResults);
        }
    }
}
