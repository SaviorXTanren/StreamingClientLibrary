using System;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Extension methods for the DateTimeOffset class.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Creates a DateTimeOffset from a UTC Unix time in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The total milliseconds in UTC Unix time</param>
        /// <returns>The equivalent DateTimeOffset</returns>
        public static DateTimeOffset FromUTCUnixTimeMilliseconds(long milliseconds) { return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).ToOffset(DateTimeOffset.Now.Offset); }

        /// <summary>
        /// Creates a DateTimeOffset from a UTC Unix time in seconds.
        /// </summary>
        /// <param name="seconds">The total secpnds in UTC Unix time</param>
        /// <returns>The equivalent DateTimeOffset</returns>
        public static DateTimeOffset FromUTCUnixTimeSeconds(long seconds) { return DateTimeOffset.FromUnixTimeSeconds(seconds).ToOffset(DateTimeOffset.Now.Offset); }

        /// <summary>
        /// Creates a DateTimeOffset from an ISO 8601 string.
        /// </summary>
        /// <param name="dateTime">The total milliseconds in ISO 8601 time</param>
        /// <returns>The equivalent DateTimeOffset</returns>
        public static DateTimeOffset FromUTCISO8601String(string dateTime) { return DateTimeOffset.Parse(dateTime).ToOffset(DateTimeOffset.Now.Offset); }

        /// <summary>
        /// Creates an ISO 8601 string.
        /// </summary>
        /// <param name="dateTime">The DateTimeOffset to convert</param>
        /// <returns>The equivalent ISO 8601 string</returns>
        public static string ToUTCISO8601String(this DateTimeOffset dateTime) { return dateTime.ToOffset(DateTimeOffset.UtcNow.Offset).ToString("s"); }

        /// <summary>
        /// Creates an RFC 3339 string
        /// </summary>
        /// <param name="dateTime">The DateTimeOffset to convert</param>
        /// <returns>The equivalent RFC 3339 string</returns>
        public static string ToRFC3339String(this DateTimeOffset dateTime) { return dateTime.ToUTCISO8601String() + "Z"; }
    }
}
