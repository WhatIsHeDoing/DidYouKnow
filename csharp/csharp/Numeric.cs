using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Demonstrating numeric value tests.
    /// </summary>
    [TestClass]
    public class Numeric
    {
        /// <summary>
        /// Shows that, if integer operations are not checked,
        /// they can overflow if the result is too large.
        /// </summary>
        [TestMethod]
        public void Unchecked()
        {
            // Brilliantly, Visual Studio will warn of:
            // "Overflow in constant value computation",
            // so cheat by converting that maximum to and from a string.
            var maxIntAsString = Convert.ToString(int.MaxValue,
                CultureInfo.CurrentCulture);
            
            var maxInt = Convert.ToInt32(maxIntAsString,
                CultureInfo.CurrentCulture);

            var result = maxInt + 10;
            Assert.IsTrue(result < 0, "We have overflow!");
        }

        /// <summary>
        /// Showing how to avoid the issue in
        /// <see cref="TestUnchecked">TestUnchecked</see> by using checked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Checked()
        {
            // Brilliantly, Visual Studio will warn of:
            // "Overflow in constant value computation",
            // so cheat by converting that maximum to and from a string.
            var maxIntAsString = Convert.ToString(int.MaxValue,
                CultureInfo.CurrentCulture);
            
            var maxInt = Convert.ToInt32(maxIntAsString,
                CultureInfo.CurrentCulture);

            var result = checked(maxInt + 10);
            Assert.Fail("Should have thrown but instead got: {0}!", result);
        }
    }
}
