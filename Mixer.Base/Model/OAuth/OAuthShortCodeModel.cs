namespace Mixer.Base.Model.OAuth
{
    public class OAuthShortCodeModel
    {
        public string code { get; set; }
        public string handle { get; set; }
        public uint expires_in { get; set; }
    }
}
