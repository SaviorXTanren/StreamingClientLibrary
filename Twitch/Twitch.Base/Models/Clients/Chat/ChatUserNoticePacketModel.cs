namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a chat user notice packet.
    /// </summary>
    public class ChatUserNoticePacketModel : ChatMessagePacketModel
    {
        /// <summary>
        /// The ID of the command for a chat user notice.
        /// </summary>
        public new const string CommandID = "USERNOTICE";

        /// <summary>
        /// The type of notice (not the ID). Valid values: sub, resub, subgift, anonsubgift, submysterygift, giftpaidupgrade, rewardgift, anongiftpaidupgrade, raid, unraid, ritual, bitsbadgetier.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// The message printed in chat along with this notice.
        /// </summary>
        public string SystemMessage { get; set; }

        /// <summary>
        /// (Sent only on sub, resub) The total number of months the user has subscribed. This is the same as msg-param-months but sent for different types of user notices.
        /// </summary>
        public int SubCumulativeMonths { get; set; }

        /// <summary>
        /// (Sent only on sub, resub, subgift, anonsubgift) The type of subscription plan being used.
        /// 
        /// Valid values: Prime, 1000, 2000, 3000. 1000, 2000, and 3000 refer to the first, second, and third levels of paid subscriptions, respectively (currently $4.99, $9.99, and $24.99).
        /// </summary>
        public string SubPlan { get; set; }

        /// <summary>
        /// (Sent only on sub, resub, subgift, anonsubgift) The display name of the subscription plan. This may be a default name or one created by the channel owner.
        /// </summary>
        public string SubPlanDisplayName { get; set; }

        /// <summary>
        /// (Sent only on sub, resub) Boolean indicating whether users want their streaks to be shared.
        /// </summary>
        public bool SubShareStreakMonths { get; set; }

        /// <summary>
        /// (Sent only on sub, resub) The number of consecutive months the user has subscribed. This is 0 if msg-param-should-share-streak is 0.
        /// </summary>
        public int SubStreakMonths { get; set; }

        /// <summary>
        /// (Sent only on giftpaidupgrade) The login of the user who gifted the subscription.
        /// </summary>
        public string SubGiftSenderLogin { get; set; }

        /// <summary>
        /// Sent only on giftpaidupgrade) The display name of the user who gifted the subscription.
        /// </summary>
        public string SubGiftSenderDisplayName { get; set; }

        /// <summary>
        /// (Sent only on subgift, anonsubgift) The total number of months the user has subscribed. This is the same as msg-param-cumulative-months but sent for different types of user notices.
        /// </summary>
        public int SubGiftMonths { get; set; }

        /// <summary>
        /// (Sent only on subgift, anonsubgift) The user ID of the subscription gift recipient.
        /// </summary>
        public string SubGiftRecipientID { get; set; }

        /// <summary>
        /// (Sent only on subgift, anonsubgift) The user name of the subscription gift recipient.
        /// </summary>
        public string SubGiftRecipientLogin { get; set; }

        /// <summary>
        /// (Sent only on subgift, anonsubgift) The display name of the subscription gift recipient.
        /// </summary>
        public string SubGiftRecipientDisplayName { get; set; }

        /// <summary>
        /// (Sent only on anongiftpaidupgrade, giftpaidupgrade) The subscriptions promo, if any, that is ongoing; e.g. Subtember 2018.
        /// </summary>
        public string SubPromoName { get; set; }

        /// <summary>
        /// (Sent only on anongiftpaidupgrade, giftpaidupgrade) The number of gifts the gifter has given during the promo indicated by msg-param-promo-name.
        /// </summary>
        public int SubPromoTotalGifts { get; set; }

        /// <summary>
        /// (Sent on only raid) The name of the source user raiding this channel.
        /// </summary>
        public string RaidUserLogin { get; set; }

        /// <summary>
        /// (Sent only on raid) The display name of the source user raiding this channel.
        /// </summary>
        public string RaidUserDisplayName { get; set; }

        /// <summary>
        /// (Sent only on raid) The number of viewers watching the source channel raiding this channel.
        /// </summary>
        public int RaidViewerCount { get; set; }

        /// <summary>
        /// (Sent only on bitsbadgetier) The tier of the bits badge the user just earned; e.g. 100, 1000, 10000.
        /// </summary>
        public long BitsTierThreshold { get; set; }

        /// <summary>
        /// (Sent only on ritual) The name of the ritual this notice is for. Valid value: new_chatter.
        /// </summary>
        public string RitualName { get; set; }

        /// <summary>
        /// Creates a new instance of the ChatUserNoticePacketModel class.
        /// </summary>
        /// <param name="packet">The Chat packet</param>
        public ChatUserNoticePacketModel(ChatRawPacketModel packet)
            : base(packet)
        {
            this.MessageID = packet.GetTagString("msg-id");
            this.SystemMessage = packet.GetTagString("system-msg");

            this.SubCumulativeMonths = packet.GetTagInt("msg-param-cumulative-months");
            this.SubPlan = packet.GetTagString("msg-param-sub-plan");
            this.SubPlanDisplayName = packet.GetTagString("msg-param-sub-plan-name");
            this.SubShareStreakMonths = packet.GetTagBool("msg-param-should-share-streak");
            this.SubStreakMonths = packet.GetTagInt("msg-param-streak-months");

            this.SubGiftSenderLogin = packet.GetTagString("msg-param-sender-login");
            this.SubGiftSenderDisplayName = packet.GetTagString("msg-param-sender-name");
            this.SubGiftMonths = packet.GetTagInt("msg-param-months");
            this.SubGiftRecipientID = packet.GetTagString("msg-param-recipient-id");
            this.SubGiftRecipientLogin = packet.GetTagString("msg-param-recipient-user-name");
            this.SubGiftRecipientDisplayName = packet.GetTagString("msg-param-recipient-display-name");

            this.SubPromoName = packet.GetTagString("msg-param-promo-name");
            this.SubPromoTotalGifts = packet.GetTagInt("msg-param-promo-gift-total");

            this.RaidUserLogin = packet.GetTagString("msg-param-login");
            this.RaidUserDisplayName = packet.GetTagString("msg-param-displayName");
            this.RaidViewerCount = packet.GetTagInt("msg-param-viewerCount");

            this.RitualName = packet.GetTagString("msg-param-ritual-name");

            this.BitsTierThreshold = packet.GetTagLong("msg-param-threshold");
        }
    }
}
