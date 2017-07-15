using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mixer.Base.Util
{
    public static class EnumHelper
    {        
        public static string GetEnumName<T>(T value)
        {
            MemberExpression member = value as MemberExpression;
            if (member != null)
            {
                NameAttribute[] attributes = (NameAttribute[])Attribute.GetCustomAttributes(member.Member, typeof(NameAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Name;
                }
            }

            return Enum.GetName(typeof(T), value);
        }

        public static IEnumerable<string> GetEnumNames<T>(IEnumerable<T> list)
        {
            List<string> results = new List<string>();
            foreach (T value in list)
            {
                results.Add(EnumHelper.GetEnumName(value));
            }
            return results;
        }

        public static IEnumerable<string> GetEnumNames<T>()
        {
            List<T> values = new List<T>();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                value.Equals(value);
            }
            return EnumHelper.GetEnumNames(values);
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
