using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.Chat
{
    public class ChatMessageUserModel
    {
        public uint user_id { get; set; }
        public string user_name { get; set; }
        public string[] user_roles { get; set; }
        public uint user_level { get; set; }
        public string user_avatar { get; set; }
    }

    public class CoordinatesModel
    {
        public uint x { get; set; }
        public uint y { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
    }

    public class ChatMessageDataModel
    {
        public string type { get; set; }
        public object data { get; set; }
        public string text { get; set; }
        public uint id { get; set; }
        public string username { get; set; }
        public string source { get; set; }
        public string pack { get; set; }
        public string url { get; set; }
        public CoordinatesModel coords { get; set; }
    }

    public class ChatMessageContentsModel
    {
        public ChatMessageDataModel[] message { get; set; }
        public JObject meta { get; set; }
    }

    public class ChatMessageEventModel : ChatMessageUserModel
    {
        public Guid id { get; set; }
        public uint channel { get; set; }
        public ChatMessageContentsModel message { get; set; }
        public string target { get; set; }
    }
}
