using System;
using System.Collections.Generic;

namespace Mixer.Base.Util
{
    public static class EnumHelper
    {
        public static string GetEnumName<T>(T value)
        {
            string name = Enum.GetName(typeof(T), value);
            if (!string.IsNullOrEmpty(name))
            {
                NameAttribute[] nameAttributes = (NameAttribute[])typeof(T).GetField(name).GetCustomAttributes(typeof(NameAttribute), false);
                if (nameAttributes != null && nameAttributes.Length > 0)
                {
                    return nameAttributes[0].Name;
                }
                return name;
            }
            return null;
        }

        public static IEnumerable<string> GetEnumNames<T>(IEnumerable<T> list)
        {
            List<string> results = new List<string>();
            foreach (T value in list)
            {
                string name = EnumHelper.GetEnumName(value);
                if (!string.IsNullOrEmpty(name))
                {
                    results.Add(name);
                }
            }
            return results;
        }

        public static IEnumerable<string> GetEnumNames<T>(bool includeObsoletes = false)
        {
            return EnumHelper.GetEnumNames(EnumHelper.GetEnumList<T>(includeObsoletes));
        }

        public static IEnumerable<T> GetEnumList<T>(bool includeObsoletes = false)
        {
            List<T> values = new List<T>();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (!includeObsoletes)
                {
                    if (EnumHelper.IsObsolete(value))
                    {
                        continue;
                    }
                }
                values.Add(value);
            }
            return values;
        }

        public static T GetEnumValueFromString<T>(string str)
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(str, EnumHelper.GetEnumName(value)))
                {
                    return value;
                }
            }
            return default(T);
        }

        public static bool IsObsolete<T>(T value)
        {
            var attributes = (ObsoleteAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return true;
            }
            return false;
        }
    }
}
