using System;

namespace Mixer.Base.Util
{
    public static class DateTimeHelper
    {
        private static readonly DateTimeOffset EpochDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, DateTimeOffset.Now.Offset);

        public static DateTimeOffset ParseUnixTimestamp(long milliseconds)
        {
            return EpochDateTimeOffset.AddMilliseconds(milliseconds);
        }
    }
}
