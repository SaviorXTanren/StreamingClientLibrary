using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mixer.Base.Util
{
    public static class JsonHelper
    {
        public static List<T> ConvertJArrayToTypedArray<T>(JArray array)
        {
            List<T> results = new List<T>();
            if (array != null)
            {
                foreach (JToken token in array)
                {
                    results.Add(token.ToObject<T>());
                }
            }
            return results;
        }

        public static T ConvertToDifferentType<T>(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            JObject jobj = JObject.Parse(json);
            return jobj.ToObject<T>();
        }
    }
}
