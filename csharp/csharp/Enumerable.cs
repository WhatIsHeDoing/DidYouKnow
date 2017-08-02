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
        static IEnumerable<string> HelloWorld()
        {
            yield return "Hello";
            yield return "World";
        }

        /// <summary>
        /// Testing generator methods via the yield keyword.
        /// </summary>
        [TestMethod]
        public void Yield()
        {
            var enumerator = HelloWorld().GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "Hello");
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "World");
        }

        class Triplets<T> : IEnumerable<IEnumerable<T>>
        {
            private readonly List<List<T>> _collection = new List<List<T>>();

            public void Add(T one, T two, T three) =>
                _collection.Add(new List<T> { one, two, three });

            public IEnumerator<IEnumerable<T>> GetEnumerator() =>
                _collection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Demonstrating that the Add method of a custom collection
        /// that implements IEnumerable is used in the object initialiser.
        /// </summary>
        [TestMethod]
        public void ObjectInitializer()
        {
            var triplets = new Triplets<int>
            {
                { 1, 2, 3 }
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            CollectionAssert.AreEqual
                (new List<int> { 1, 2, 3 },
                    triplets.FirstOrDefault().ToList());
        }

        /// <summary>
        /// Simply showing how jagged arrays - arrays of arrays
        /// of unequal length - can be flattened using SelectMany.
        /// </summary>
        [TestMethod]
        public void JaggedArrays()
        {
            // Assemble.
            var jagged = new[]
            {
                new [] { 1, 2 },
                new [] { 3, 4, 5 },
                new [] { 6 }
            };

            var expected = new[] { 1, 2, 3, 4, 5, 6 };

            // Act.
            var flattened = jagged.SelectMany(a => a.Select(e => e)).ToArray();

            // Assert.
            CollectionAssert.AreEqual(expected, flattened);
        }
    }
}
