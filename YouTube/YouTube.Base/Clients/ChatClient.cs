using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YouTube.Base.Model;
using YouTube.Base.Services;

namespace YouTube.Base.Clients
{
    /// <summary>
    /// Handles interaction with Live Stream-based chat services.
    /// </summary>
    public class ChatClient : YouTubeServiceBase, IDisposable
    {
        private LiveBroadcast broadcast;

        private CancellationTokenSource messageBackgroundPollingTokenSource;

        private HashSet<string> messageIDs = new HashSet<string>();

        /// <summary>
        /// Invoked when chat messages are received.
        /// </summary>
        public event EventHandler<IEnumerable<LiveChatMessage>> OnMessagesReceived;

        /// <summary>
        /// Creates a new instance of the ChatClient class.
        /// </summary>
        /// <param name="connection">The connection to YouTube</param>
        public ChatClient(YouTubeConnection connection) : base(connection) { }

        /// <summary>
        /// Connects to the chat for the current broadcast.
        /// </summary>
        /// <returns>Whether the connection was successful</returns>
        public async Task<bool> Connect()
        {
            this.broadcast = await this.connection.LiveBroadcasts.GetActiveBroadcast();

            this.messageBackgroundPollingTokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(this.MessageBackgroundPolling, this.messageBackgroundPollingTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return true;
        }

        /// <summary>
        /// Disconnectes from the current broadcast.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public Task Disconnect()
        {
            this.broadcast = null;
            if (this.messageBackgroundPollingTokenSource != null)
            {
                this.messageBackgroundPollingTokenSource.Cancel();
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sends a message to the live chat.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>The resulting message</returns>
        public async Task<LiveChatMessage> SendMessage(string message) { return await this.connection.LiveChat.SendMessage(this.broadcast, message); }

        /// <summary>
        /// Deletes the specified message from the live chat.
        /// </summary>
        /// <param name="message">The message to delete</param>
        /// <returns>An awaitable Task</returns>
        public async Task DeleteMessage(LiveChatMessage message) { await this.connection.LiveChat.DeleteMessage(message); }

        /// <summary>
        /// Gets the most recent super chat events for a live chat.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of super chat events</returns>
        public async Task<IEnumerable<SuperChatEvent>> GetRecentSuperChatEvents(int maxResults = 1) { return await this.connection.LiveChat.GetRecentSuperChatEvents(this.broadcast, maxResults); }

        /// <summary>
        /// Gets the list of channel memberships.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of channel memberships</returns>
        public async Task<IEnumerable<Member>> GetChannelMemberships(int maxResults = 1) { return await this.connection.LiveChat.GetChannelMemberships(maxResults); }

        /// <summary>
        /// Gets the list of moderators.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of moderators</returns>
        public async Task<IEnumerable<LiveChatModerator>> GetModerators(int maxResults = 1) { return await this.connection.LiveChat.GetModerators(this.broadcast, maxResults); }

        /// <summary>
        /// Makes the specified user a moderator.
        /// </summary>
        /// <param name="user">The user to mod</param>
        /// <returns>Information about the modded user</returns>
        public async Task<LiveChatModerator> ModUser(Channel user) { return await this.connection.LiveChat.ModUser(this.broadcast, user); }

        /// <summary>
        /// Removes moderator privileges from the specified user.
        /// </summary>
        /// <param name="moderator">The user to unmode</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnmodUser(LiveChatModerator moderator) { await this.connection.LiveChat.UnmodUser(moderator); }

        /// <summary>
        /// Times out the specified user from the channel.
        /// </summary>
        /// <param name="user">The user to timeout</param>
        /// <param name="duration">The length of the timeout in seconds</param>
        /// <returns>The timeout result</returns>
        public async Task<LiveChatBan> TimeoutUser(Channel user, ulong duration) { return await this.connection.LiveChat.TimeoutUser(this.broadcast, user, duration); }

        /// <summary>
        /// Bans the specified user from the channel.
        /// </summary>
        /// <param name="user">The user to ban</param>
        /// <returns>The ban result</returns>
        public async Task<LiveChatBan> BanUser(Channel user) { return await this.connection.LiveChat.BanUser(this.broadcast, user); }

        /// <summary>
        /// Unbans the specified user from the channel.
        /// </summary>
        /// <param name="ban">The ban to remove</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnbanUser(LiveChatBan ban) { await this.connection.LiveChat.UnbanUser(ban); }

        private async Task<LiveChatBan> BanUserInternal(LiveBroadcast broadcast, Channel user, string banType, ulong banDuration = 0)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                LiveChatBan ban = new LiveChatBan();
                ban.Snippet = new LiveChatBanSnippet();
                ban.Snippet.LiveChatId = broadcast.Snippet.LiveChatId;
                ban.Snippet.Type = banType;
                if (banDuration > 0)
                {
                    ban.Snippet.BanDurationSeconds = banDuration;
                }
                ban.Snippet.BannedUserDetails = new ChannelProfileDetails();
                ban.Snippet.BannedUserDetails.ChannelId = user.Id;

                LiveChatBansResource.InsertRequest request = this.connection.GoogleYouTubeService.LiveChatBans.Insert(ban, "snippet");
                return await request.ExecuteAsync();
            });
        }

        private async Task MessageBackgroundPolling()
        {
            string nextResultsToken = null;
            while (!this.messageBackgroundPollingTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (broadcast != null)
                    {
                        LiveChatMessagesResultModel result = await this.connection.LiveChat.GetMessages(this.broadcast, nextResultsToken: nextResultsToken, maxResults: 200);
                        if (result != null)
                        {
                            List<LiveChatMessage> newMessages = new List<LiveChatMessage>();
                            foreach (LiveChatMessage message in result.Messages)
                            {
                                if (!messageIDs.Contains(message.Id))
                                {
                                    newMessages.Add(message);
                                    messageIDs.Add(message.Id);
                                }
                            }

                            if (newMessages.Count > 0)
                            {
                                this.OnMessagesReceived?.Invoke(this, newMessages);
                            }

                            nextResultsToken = result.NextResultsToken;
                            await Task.Delay((int)result.PollingInterval);
                        }
                        else
                        {
                            await Task.Delay(10000);
                        }
                    }
                    else
                    {
                        this.broadcast = await this.connection.LiveBroadcasts.GetActiveBroadcast();
                        await Task.Delay(60000);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { Logger.Log(ex); }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes the resources of the object.
        /// </summary>
        /// <param name="disposing">Whether disposal is taking place</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    this.messageBackgroundPollingTokenSource.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                // Set large fields to null.

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the resources of the object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
