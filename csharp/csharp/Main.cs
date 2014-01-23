using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Run me with Test => Run => All Tests
    /// </summary>
    [TestClass]
    public class Main
    {
        /// <summary>
        /// Demonstrates chaining the null-coalescing operator
        /// </summary>
        /// <remarks>http://msdn.microsoft.com/en-us/library/ms173224.aspx</remarks>
        [TestMethod]
        public void TestChainingNullCoalescingOperator()
        {
            int? firstNull = null;
            int? secondNull = null;
            Assert.AreEqual(firstNull ?? secondNull ?? 123, 123);
        }
    }
}
