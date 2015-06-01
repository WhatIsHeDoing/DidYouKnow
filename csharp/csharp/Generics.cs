using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Alias used in a test
using StringToString = System.Collections.Generic.Dictionary<string, string>;

namespace csharp
{
    /// <summary>
    /// Some interesting uses and features of generics.
    /// </summary>
    [TestClass]
    public class Generics
    {
        /// <summary>
        /// Shows how an alias to a generic can be used
        /// to potentially simplify the use of complex structures.
        /// </summary>
        [TestMethod]
        public void TestAliasedGeneric()
        {
            var aliasedMap = new StringToString
            {
                { "Darren", "Hickling" }
            };

            var map = new Dictionary<string, string>
            {
                { "Darren", "Hickling" }
            };

            Assert.AreEqual(aliasedMap["Darren"], map["Darren"]);
        }

        T returnValue<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Why explicitly state the type,
        /// when the compiler can determine it for you?
        /// </summary>
        [TestMethod]
        public void TestImplicitGenerics()
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            Assert.AreEqual(returnValue<int>(23), returnValue(23));
        }

        T getDefaultValue<T>()
        {
            return default(T);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class Blank { }

        /// <summary>
        /// Shows how the default value of generic works as expected
        /// with value (including nullable) and reference types.
        /// </summary>
        [TestMethod]
        public void TestDefaultValue()
        {
            Assert.IsFalse(getDefaultValue<bool>());
            Assert.AreEqual(getDefaultValue<int>(), 0);
            Assert.IsNull(getDefaultValue<double?>());
            Assert.IsNull(getDefaultValue<List<string>>());
            Assert.IsNull(getDefaultValue<Blank>());
        }
    }
}
