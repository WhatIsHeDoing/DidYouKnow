using Xunit;

namespace csharp
{
    /// <summary>
    /// Demonstrates how to use lazy instantiation.
    /// </summary>
    public class LazyInstantiation
    {
        class EpicConstruction
        {
            public readonly bool IsSetup;

            public EpicConstruction()
            {
                Thread.Sleep(1000);
                IsSetup = true;
            }
        }

        /// <summary>
        /// Shows how lazy instantiation delays the
        /// construction of an object until it is used.
        /// </summary>
        [Fact]
        public void LazyTest()
        {
            var start = DateTime.Now;

            var lazy = new Lazy<EpicConstruction>();
            Assert.False(lazy.IsValueCreated);

            var lazyTime = DateTime.Now - start;

            start = DateTime.Now;
            var lazyEpicConstruction = lazy.Value;
            var lazyAccessTime = DateTime.Now - start;

            Assert.True(lazy.IsValueCreated);
            Assert.True(lazyEpicConstruction.IsSetup);
            Assert.True(lazyTime < lazyAccessTime);

            start = DateTime.Now;
            var epicConstruction = new EpicConstruction();
            var expensiveEndTime = DateTime.Now - start;

            Assert.True(epicConstruction.IsSetup);
            Assert.True(lazyTime < expensiveEndTime);
        }
    }
}
