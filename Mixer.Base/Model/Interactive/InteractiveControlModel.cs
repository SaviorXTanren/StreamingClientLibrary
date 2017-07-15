using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveControlModel
    {
        public string controlID { get; set; }
        public string kind { get; set; }
        public int keycode { get; set; }
        public string text { get; set; }
        public int cost { get; set; }
        public long cooldown { get; set; }
        public string etag { get; set; }

     
        public double progress { get; set; }
        public bool disabled { get; set; }
        public JObject meta { get; set; } = new JObject();
    }
}
