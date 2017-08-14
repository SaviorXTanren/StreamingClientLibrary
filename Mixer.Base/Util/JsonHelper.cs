using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Util
{
    public static class JsonHelper
    {
        public static T ConvertToDifferentType<T>(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            JObject jobj = JObject.Parse(json);
            return jobj.ToObject<T>();
        }
    }
}
