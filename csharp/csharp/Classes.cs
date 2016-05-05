using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace csharp
{
    [TestClass]
    public class Classes
    {
        class TestClass
        {
            private readonly IDictionary<string, int> _dictionary;

            public TestClass(IDictionary<string, int> dictionary)
            {
                _dictionary = dictionary;
            }

            public int this[string key]
            {
                get { return _dictionary[key]; }
            }

            public IEnumerable<int> this[params string[] keys]
            {
                get { return keys.Select(key => _dictionary[key]); }
            }

            public string this[int key]
            {
                get { return _dictionary.First(kv => kv.Value == key).Key; }
            }
        }

        [TestMethod]
        public void MultipleIndexAccessors()
        {
            var testClass = new TestClass(new Dictionary<string, int>
            {
                { "a", 1 },
                { "b", 2 },
                { "c", 3 }
            });

            Assert.AreEqual(2, testClass["b"]);

            var actual = testClass["b", "c"];
            CollectionAssert.AreEqual(new[] { 2, 3 }, actual.ToArray());
        }

        [TestMethod]
        public void DifferentIndexAccessorTypes()
        {
            var testClass = new TestClass(new Dictionary<string, int>
            {
                { "a", 1 },
                { "b", 2 },
                { "c", 3 }
            });

            Assert.AreEqual(2, testClass["b"]);
            Assert.AreEqual("b", testClass[2]);
        }
    }
}
