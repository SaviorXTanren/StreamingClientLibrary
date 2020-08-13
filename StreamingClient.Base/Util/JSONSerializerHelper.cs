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
        /// <param name="includeObjectType">Whether to include the serialized object type in the serialized string</param>
        /// <returns>The serialized string</returns>
        public static string SerializeToString<T>(T data, bool includeObjectType = true)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { TypeNameHandling = (includeObjectType) ? TypeNameHandling.Objects : TypeNameHandling.None });
        }

        /// <summary>
        /// Deserialized the specified string to a typed object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The string to deserialize</param>
        /// <param name="ignoreErrors">Whether to ignore deserialization errors</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromString<T>(string data, bool ignoreErrors = false)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            };

            if (ignoreErrors)
            {
                serializerSettings.Error = IgnoreDeserializationError;
            }

            return JsonConvert.DeserializeObject<T>(data, serializerSettings);
        }

        /// <summary>
        /// Deserialized the specified string to an abstract typed object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="data">The string to deserialize</param>
        /// <param name="ignoreErrors">Whether to ignore deserialization errors</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeAbstractFromString<T>(string data, bool ignoreErrors = false)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            };

            if (ignoreErrors)
            {
                serializerSettings.Error = IgnoreDeserializationError;
            }

            return (T)JsonConvert.DeserializeObject(data, serializerSettings);
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

        private static void IgnoreDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            Logger.Log(errorArgs.ErrorContext.Error.Message);
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
