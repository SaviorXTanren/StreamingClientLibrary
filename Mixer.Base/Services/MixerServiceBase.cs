using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public abstract class MixerServiceBase : RestServiceBase
    {
        private const string MixerRestAPIBaseAddress = "https://mixer.com/api/v1/";

        private MixerConnection connection;

        public MixerServiceBase(MixerConnection connection)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
        }

        internal MixerServiceBase() { }

        protected override async Task<OAuthTokenModel> GetOAuthToken()
        {
            if (this.connection != null)
            {
                return await this.connection.GetOAuthToken();
            }
            return null;
        }

        protected override string GetBaseAddress() { return MixerServiceBase.MixerRestAPIBaseAddress; }
    }
}
