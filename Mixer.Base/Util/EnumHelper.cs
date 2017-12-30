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
                NameAttribute[] attributes = (NameAttribute[])typeof(T).GetField(name).GetCustomAttributes(typeof(NameAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Name;
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

        public static IEnumerable<string> GetEnumNames<T>()
        {
            return EnumHelper.GetEnumNames(EnumHelper.GetEnumList<T>());
        }

        public static IEnumerable<T> GetEnumList<T>()
        {
            List<T> values = new List<T>();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
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
    }
}
