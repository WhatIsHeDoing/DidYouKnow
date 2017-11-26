using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace csharp
{
    /// <summary>
    /// Demonstrating little-known features of classes.
    /// </summary>
    public class Classes
    {
        class TestClass
        {
            private readonly IDictionary<string, int> _dictionary;

            public TestClass(IDictionary<string, int> dictionary)
                => _dictionary = dictionary;

            public int this[string key] => _dictionary[key];

            public IEnumerable<int> this[params string[] keys]
                => keys.Select(key => _dictionary[key]);

            public string this[int key]
                => _dictionary.First(kv => kv.Value == key).Key;
        }

        /// <summary>
        /// Shows how multiple array indexes can be accessed simultaneously.
        /// </summary>
        [Fact]
        public void MultipleIndexAccessors()
        {
            var testClass = new TestClass(new Dictionary<string, int>
            {
                { "a", 1 },
                { "b", 2 },
                { "c", 3 }
            });

            Assert.Equal(2, testClass["b"]);

            var actual = testClass["b", "c"];
            Assert.Equal(new[] { 2, 3 }, actual.ToArray());
        }

        /// <summary>
        /// Shows that different types can be accessed via the same syntax.
        /// </summary>
        [Fact]
        public void DifferentIndexAccessorTypes()
        {
            var testClass = new TestClass(new Dictionary<string, int>
            {
                { "a", 1 },
                { "b", 2 },
                { "c", 3 }
            });

            Assert.Equal(2, testClass["b"]);
            Assert.Equal("b", testClass[2]);
        }
    }
}
