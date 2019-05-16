using Newtonsoft.Json;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Helper class for handling JSON serialization
    /// </summary>
    public static class JSONSerializerHelper
    {
        /// <summary>
        /// Serializes the specified object to a string
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The object to serialize</param>
        /// <returns>The serialized string</returns>
        public static string SerializeToString<T>(T data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Deserialized the specified string to a typed object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The string to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Deserialized the specified string to an abstract typed object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The string to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeAbstractFromString<T>(string data)
        {
            return (T)JsonConvert.DeserializeObject(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Clones the specified object by serializing &amp; deserializing it.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The object to clone</param>
        /// <returns>The cloned object</returns>
        public static T Clone<T>(object data)
        {
            return JSONSerializerHelper.DeserializeFromString<T>(JSONSerializerHelper.SerializeToString(data));
        }
    }
}
