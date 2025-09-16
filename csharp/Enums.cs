using Xunit;

namespace csharp
{
    /// <summary>
    /// Simple duration enum.
    /// </summary>
    public enum Duration { Day, Week, Month };

    /// <summary>
    /// Extension methods for the enum above.
    /// </summary>
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
                    throw new ArgumentOutOfRangeException(nameof(duration));
            }
        }
    }

    /// <summary>
    /// Fun with enums.
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// Demonstrates how extension methods can be applied to enums.
        /// </summary>
        [Fact]
        public void CanUseExtensionMethods()
        {
            var startDate = new DateTime(2000, 1, 1);
            var actual = Duration.Day.From(startDate);
            var expected = new DateTime(2000, 1, 2);
            Assert.Equal(expected, actual);
        }
    }
}
