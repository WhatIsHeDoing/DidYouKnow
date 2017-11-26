using System;
using System.Globalization;
using Xunit;

namespace csharp
{
    /// <summary>
    /// Demonstrating numeric value tests.
    /// </summary>
    public class Numeric
    {
        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        /// <summary>
        /// Shows that, if integer operations are not checked,
        /// they can overflow if the result is too large.
        /// </summary>
        [Fact]
        public void Unchecked()
        {
            // Brilliantly, Visual Studio will warn of:
            // "Overflow in constant value computation",
            // so cheat by converting that maximum to and from a string.
            var maxIntAsString = Convert.ToString(int.MaxValue, _culture);
            var maxInt = Convert.ToInt32(maxIntAsString, _culture);
            var result = maxInt + 10;
            Assert.True(result < 0);
        }

        /// <summary>
        /// Showing how to avoid the issue in
        /// <see cref="Unchecked">TestUnchecked</see> by using checked.
        /// </summary>
        [Fact]
        public void Checked()
        {
            // Brilliantly, Visual Studio will warn of:
            // "Overflow in constant value computation",
            // so cheat by converting that maximum to and from a string.
            var maxIntAsString = Convert.ToString(int.MaxValue, _culture);
            var maxInt = Convert.ToInt32(maxIntAsString, _culture);
            Assert.Throws<OverflowException>(() => checked(maxInt + 10));
        }
    }
}
