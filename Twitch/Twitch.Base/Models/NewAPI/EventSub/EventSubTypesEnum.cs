using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch.Base.Models.NewAPI.EventSub
{
    /// <summary>
    /// Twitch provided event sub types.
    /// </summary>
    public enum EventSubTypesEnum
    {
        //
        // V1

        /// <summary>
        /// A broadcaster updates their channel properties e.g., category, title, mature flag, broadcast, or language.
        /// </summary>
        [Name("channel.update")]
        ChannelUpdate,
        /// <summary>
        /// A specified channel receives a follow.
        /// </summary>
        [Name("channel.follow")]
        ChannelFollow,
        /// <summary>
        /// A notification when a specified channel receives a subscriber. This does not include resubscribes.
        /// </summary>
        [Name("channel.subscribe")]
        ChannelSubscribe,
        /// <summary>
        /// A user cheers on the specified channel.
        /// </summary>
        [Name("channel.cheer")]
        ChannelCheer,
        /// <summary>
        /// A broadcaster raids another broadcaster’s channel.
        /// </summary>
        [Name("channel.raid")]
        ChannelRaid,
        /// <summary>
        /// A viewer is banned from the specified channel.
        /// </summary>
        [Name("channel.ban")]
        ChannelBan,
        /// <summary>
        /// A viewer is unbanned from the specified channel.
        /// </summary>
        [Name("channel.unban")]
        ChannelUnban,
        /// <summary>
        /// Moderator privileges were added to a user on a specified channel.
        /// </summary>
        [Name("channel.moderator.add")]
        ChannelModeratorAdd,
        /// <summary>
        /// Moderator privileges were removed from a user on a specified channel.
        /// </summary>
        [Name("channel.moderator.remove")]
        ChannelModeratorRemove,
        /// <summary>
        /// A custom channel points reward has been created for the specified channel.
        /// </summary>
        [Name("channel.channel_points_custom_reward.add")]
        ChannelPointsCustomRewardAdd,
        /// <summary>
        /// A custom channel points reward has been updated for the specified channel.
        /// </summary>
        [Name("channel.channel_points_custom_reward.update")]
        ChannelPointsCustomRewardUpdate,
        /// <summary>
        /// A custom channel points reward has been removed from the specified channel.
        /// </summary>
        [Name("channel.channel_points_custom_reward.remove")]
        ChannelPointsCustomRewardRemove,
        /// <summary>
        /// A viewer has redeemed a custom channel points reward on the specified channel.
        /// </summary>
        [Name("channel.channel_points_custom_reward_redemption.add")]
        ChannelPointsCustomRewardRedemptionAdd,
        /// <summary>
        /// A redemption of a channel points custom reward has been updated for the specified channel.
        /// </summary>
        [Name("channel.channel_points_custom_reward_redemption.update")]
        ChannelPointsCustomRewardRedemptionUpdate,
        /// <summary>
        /// A hype train begins on the specified channel.
        /// </summary>
        [Name("channel.hype_train.begin")]
        ChannelHypeTrainBegin,
        /// <summary>
        /// A hype train makes progress on the specified channel.
        /// </summary>
        [Name("channel.hype_train.progress")]
        ChannelHypeTrainProgress,
        /// <summary>
        /// A hype train ends on the specified channel.
        /// </summary>
        [Name("channel.hype_train.end")]
        ChannelHypeTrainEnd,
        /// <summary>
        /// The specified broadcaster starts a stream.
        /// </summary>
        [Name("stream.online")]
        StreamOnline,
        /// <summary>
        /// The specified broadcaster stops a stream.
        /// </summary>
        [Name("stream.offline")]
        StreamOffline,
        /// <summary>
        /// A user’s authorization has been revoked for your client id.
        /// </summary>
        [Name("user.authorization.revoke")]
        UserAuthorizationRevoke,
        /// <summary>
        /// A user has updated their account.
        /// </summary>
        [Name("user.update")]
        UserUpdate,
    }
}
