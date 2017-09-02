using System;

namespace Mixer.Base.Util
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset UnixTimestampToDateTimeOffset(long milliseconds) { return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).ToOffset(DateTimeOffset.Now.Offset); }

        public static long DateTimeOffsetToUnixTimestamp(DateTimeOffset dateTime) { return dateTime.ToUnixTimeMilliseconds(); }

        public static string DateTimeOffsetToISO8601String(DateTimeOffset dateTime) { return dateTime.ToOffset(DateTimeOffset.UtcNow.Offset).ToString("s"); }

        public static DateTimeOffset ISO8601StringToDateTimeOffset(string dateTime) { return DateTimeOffset.Parse(dateTime).ToOffset(DateTimeOffset.Now.Offset); }
    }
}
