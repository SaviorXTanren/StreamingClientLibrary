using System;
using System.Text;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Extension methods for the String class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to it's base64 equivalent string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64String(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
    }
}
