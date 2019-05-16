using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.Costream
{
    public class CostreamModel
    {
        public CostreamChannelModel[] channels { get; set; }
        public DateTimeOffset startedAt { get; set; }
        public uint capacity { get; set; }
        public CostreamLayoutModel layout { get; set; }
    }
}
