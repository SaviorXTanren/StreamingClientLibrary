using System;
using System.Collections.Generic;
using System.Resources;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Helper class for interacting with enums.
    /// </summary>
    public static class EnumHelper
    {
        private static ResourceManager enumLocalizationResourceManager;

        /// <summary>
        /// Gets the public-facing name of the specified enum from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The public-facing name of the specified enum</returns>
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

        /// <summary>
        /// Gets the public-facing name of the specified enums from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="list">The enum values</param>
        /// <returns>The public-facing name of the specified enums</returns>
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

        /// <summary>
        /// Gets the public-facing name of the specified enums from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="includeObsoletes">Whether to include obsolete enums</param>
        /// <returns>The public-facing name of the specified enums</returns>
        public static IEnumerable<string> GetEnumNames<T>(bool includeObsoletes = false)
        {
            return EnumHelper.GetEnumNames(EnumHelper.GetEnumList<T>(includeObsoletes));
        }

        /// <summary>
        /// Gets a list of all available values for an enum type.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="includeObsoletes">Whether to include obsolete enums</param>
        /// <returns>The list of all available values for the enum type</returns>
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

        /// <summary>
        /// Gets the enum value that matches the specified name.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="str">The name to search for</param>
        /// <returns>The enum value that matches the specified name</returns>
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

        /// <summary>
        /// Indicates whether an enum value is obsolete.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>Whether it is obsolete</returns>
        public static bool IsObsolete<T>(T value)
        {
            var attributes = (ObsoleteAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the resource manager to use for localization of names for enum values.
        /// </summary>
        /// <param name="enumLocalizationResourceManager">The localized resource manager</param>
        public static void SetEnumLocalizationResourceManager(ResourceManager enumLocalizationResourceManager)
        {
            EnumHelper.enumLocalizationResourceManager = enumLocalizationResourceManager;
        }

        /// <summary>
        /// Gets the localized, public-facing name of the specified enum.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The localized, public-facing name of the specified enum</returns>
        public static string GetLocalizedName<T>(T value)
        {
            string name = EnumHelper.GetEnumName<T>(value);
            if (EnumHelper.enumLocalizationResourceManager != null)
            {
                return EnumHelper.enumLocalizationResourceManager.GetString(name) ?? name;
            }
            return name;
        }
    }
}
