﻿using System;

namespace EduardoBotv2.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDurationString(this TimeSpan ts) => $"{Math.Truncate(ts.TotalMinutes):00}:{ts.Seconds:00}";
    }
}