using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Fun with strings.
    /// </summary>
    [TestClass]
    public class Strings
    {
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
