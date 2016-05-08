using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace csharp
{
    /// <summary>
    /// Fun with strings.
    /// </summary>
    [TestClass]
    public class Strings
    {
        /// <summary>
        /// Ignoring escape patterns and respecting formatting, thanks to
        /// verbatim strings. Note that a whitespace-replaced version is
        /// tested, as the line formatting of this file can vary.
        /// Use <code>System.Environment.NewLine</code> for consistency!
        /// </summary>
        [TestMethod]
        public void VerbatimString()
        {
            const string verbatim = @"Hello
    World";

            var actual = Regex.Replace(verbatim, @"\s", "");
            const string expected = "HelloWorld";
            Assert.AreEqual(expected, actual);
        }

        class TestClass
        {
            public static string StaticString = "remembered";
            public const string ConstString = "remembered";
            public string InstanceString = "remembered";
        }

        /// <summary>
        /// Demonstrating that strings from all scopes are interned.
        /// </summary>
        [TestMethod]
        public void StringInterningByDefault()
        {
            var test = new TestClass();
            var internedStaticString = string.IsInterned(TestClass.StaticString);
            var internedConstString = string.IsInterned(TestClass.ConstString);
            var internedInstanceString = string.IsInterned(test.InstanceString);

            Assert.IsTrue(ReferenceEquals(internedStaticString, "remembered"));
            Assert.IsTrue(ReferenceEquals(internedStaticString, internedConstString));
            Assert.IsTrue(ReferenceEquals(internedStaticString, internedInstanceString));
            Assert.IsTrue(ReferenceEquals(internedConstString, internedInstanceString));

            Assert.IsFalse(ReferenceEquals(internedStaticString, "different!"));
        }
    }
}
