using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Skills;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for skill-based services.
    /// </summary>
    public class SkillsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the SkillsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public SkillsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the skill catalog of the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the skill catalog for</param>
        /// <param name="level">The level of a user who wishes to use the skills</param>
        /// <returns>The skill catalogs</returns>
        public async Task<SkillCatalogModel> GetSkillCatalog(ChannelModel channel, int level = 0)
        {
            Validator.ValidateVariable(channel, "channel");

            JObject payload = new JObject() { { "globalLevel", level } };
            return await this.PostAsync<SkillCatalogModel>("https://mixer.com/api/v1/catalog/skills/channels/" + channel.id + "?locale=en-US", this.CreateContentFromObject(payload));
        }
    }
}
