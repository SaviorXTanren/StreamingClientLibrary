using System;
using System.Collections.Generic;

namespace Mixer.Base.Util
{
    public static class EnumHelper
    {
        public static IEnumerable<string> EnumListToStringList<T>(IEnumerable<T> list)
        {
            List<string> results = new List<string>();
            foreach (T value in list)
            {
                results.Add(EnumHelper.EnumToString(value));
            }
            return results;
        }
        
        public static string EnumToString<T>(T value)
        {
            string enumName = Enum.GetName(typeof(T), value);
            if (string.IsNullOrEmpty(enumName))
            {
                throw new ArgumentException("Invalid enum specified: " + value);
            }
            return enumName;
        }
    }
}
