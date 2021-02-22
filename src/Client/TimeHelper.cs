using System;

namespace Bytewizer.TinyCLR.DigitalPortal.Client
{
    internal class TimeHelper
    {
        //public static long ToUnixTimestamp(DateTime target)
        //{
        //    var date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
        //    var unixTimestamp = Convert.ToInt64((target - date).TotalSeconds);

        //    return unixTimestamp;
        //}

        public static DateTime ToDateTime(int timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0);

            return dateTime.AddSeconds(timestamp);
        }
    }
}
