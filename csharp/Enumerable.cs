using System.Collections;
using Xunit;

namespace csharp
{
    /// <summary>
    /// Demonstrating features of enumerable collections.
    /// </summary>
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
        [Fact]
        public void Yield()
        {
            var enumerator = HelloWorld().GetEnumerator();

            enumerator.MoveNext();
            Assert.Equal("Hello", enumerator.Current);

            enumerator.MoveNext();
            Assert.Equal("World", enumerator.Current);
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
        [Fact]
        public void ObjectInitializer()
        {
            var triplets = new Triplets<int>
            {
                { 1, 2, 3 }
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            var firstTriplet = triplets.FirstOrDefault();
            Assert.NotNull(firstTriplet);

            Assert.Equal(
                [1, 2, 3],
                firstTriplet.ToList());
        }

        /// <summary>
        /// Simply showing how jagged arrays - arrays of arrays
        /// of unequal length - can be flattened using SelectMany.
        /// </summary>
        [Fact]
        public void JaggedArrays()
        {
            // Assemble.
            var jagged = new int[][]
            {
                new [] { 1, 2 },
                new [] { 3, 4, 5 },
                new [] { 6 }
            };

            var expected = new[] { 1, 2, 3, 4, 5, 6 };

            // Act.
            var flattened = jagged.SelectMany(a => a.Select(e => e)).ToArray();

            // Assert.
            Assert.Equal(expected, flattened);
        }
    }
}
