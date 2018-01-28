using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TimeEx
{
    public static double getTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return ts.TotalSeconds;
    }

}
