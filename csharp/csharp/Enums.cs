using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace csharp
{
    public enum Duration { Day, Week, Month };

    static class DurationExtensions
    {
        public static DateTime From(this Duration duration, DateTime dateTime)
        {
            switch (duration)
            {
                case Duration.Day:
                    return dateTime.AddDays(1);

                case Duration.Week:
                    return dateTime.AddDays(7);

                case Duration.Month:
                    return dateTime.AddMonths(1);

                default:
                    throw new ArgumentOutOfRangeException("duration");
            }
        }
    }

    [TestClass]
    public class Enums
    {
        [TestMethod]
        public void CanUseExtensionMethods()
        {
            var startDate = new DateTime(2000, 1, 1);
            var actual = Duration.Day.From(startDate);
            var expected = new DateTime(2000, 1, 2);
            Assert.AreEqual(expected, actual);
        }
    }
}
