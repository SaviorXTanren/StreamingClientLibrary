using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trovo.Base.Models.Chat
{
    /// <summary>
    /// Viewer information for a channel.
    /// </summary>
    [DataContract]
    public class ChatViewersModel
    {
        /// <summary>
        /// The channel's streamer nickname.
        /// </summary>
        [DataMember]
        public string nickname { get; set; }

        /// <summary>
        /// The channel's title.
        /// </summary>
        [DataMember]
        public string live_title { get; set; }

        /// <summary>
        /// The channel's total login users.
        /// </summary>
        [DataMember]
        public string total { get; set; }

        /// <summary>
        /// An map of online users nickname group by role type.
        /// </summary>
        [DataMember]
        public ChatViewersRolesModel chatters { get; set; } = new ChatViewersRolesModel();

        /// <summary>
        /// An map of online users nickname group by custome role type.
        /// </summary>
        [DataMember]
        public JObject custom_roles { get; set; } = new JObject();

        /// <summary>
        /// An map of online users nickname group by custome role type.
        /// </summary>
        [DataMember]
        public JObject custome_roles { get; set; } = new JObject();

        /// <summary>
        /// The total number of pages of this multi-page response.
        /// </summary>
        [DataMember]
        public int total_page { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        [DataMember]
        public int cursor { get; set; }
    }

    /// <summary>
    /// Role viewers for a channel.
    /// </summary>
    [DataContract]
    public class ChatViewersRolesModel
    {
        /// <summary>
        /// The VIP viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel VIPS { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The ace viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel ace { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The aceplus viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel aceplus { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The admins viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel admins { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The all viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel all { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The creators viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel creators { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The editors viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel editors { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The followers viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel followers { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The moderators viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel moderators { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The subscribers viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel subscribers { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The supermods viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel supermods { get; set; } = new ChatViewersRoleGroupModel();

        /// <summary>
        /// The wardens viewers.
        /// </summary>
        [DataMember]
        public ChatViewersRoleGroupModel wardens { get; set; } = new ChatViewersRoleGroupModel();
    }

    /// <summary>
    /// Viewers for a specific role.
    /// </summary>
    [DataContract]
    public class ChatViewersRoleGroupModel
    {
        /// <summary>
        /// The list of viewers.
        /// </summary>
        [DataMember]
        public List<string> viewers { get; set; } = new List<string>();
    }
}
