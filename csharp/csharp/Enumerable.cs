using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Demonstrating features of enumerable collections.
    /// </summary>
    [TestClass]
    public class Enumerable
    {
        IEnumerable<String> helloWorld()
        {
            yield return "Hello";
            yield return "World";
            yield break;
        }

        /// <summary>
        /// Testing generator methods via the yield keyword.
        /// </summary>
        [TestMethod]
        public void TestYield()
        {
            var enumerator = helloWorld().GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "Hello");
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "World");
        }

        class Triplets<T> : IEnumerable<IEnumerable<T>>
        {
            protected List<List<T>> _collection = new List<List<T>>();

            public void Add(T one, T two, T three)
            {
                _collection.Add(new List<T> { one, two, three });
            }

            public IEnumerator<IEnumerable<T>> GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Demonstrating that the Add method of a custom collection
        /// that implements IEnumerable is used in the object initialiser.
        /// </summary>
        [TestMethod]
        public void TestObjectInitialiser()
        {
            var triplets = new Triplets<int>
            {
                { 1, 2, 3 }
            };

            CollectionAssert.AreEqual
                (new List<int> { 1, 2, 3 },
                    triplets.FirstOrDefault().ToList());
        }
    }
}
