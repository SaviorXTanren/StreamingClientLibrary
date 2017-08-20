using System;

namespace Mixer.Base.Util
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset UnixTimestampToDateTimeOffset(long milliseconds) { return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).ToOffset(DateTimeOffset.Now.Offset); }

        public static long DateTimeOffsetToUnixTimestamp(DateTimeOffset dateTime) { return dateTime.ToUnixTimeMilliseconds(); }
    }
}
