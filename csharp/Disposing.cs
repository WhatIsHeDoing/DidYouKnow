using Xunit;

namespace csharp
{
    /// <summary>
    /// Shows how using ensures the correct use of IDisposable objects.
    /// </summary>
    public class Disposing
    {
        class Resource
        {
            public bool IsDisposed { get; set; }

            public Resource() =>
                IsDisposed = false;
        }

        class DisposingClass : IDisposable
        {
            private readonly Resource _resource;

            public DisposingClass(Resource resource) =>
                _resource = resource;

            void IDisposable.Dispose() =>
                _resource.IsDisposed = true;
        }

        /// <summary>
        /// Demonstrates how the using statement defines a scope for an object
        /// automatically disposes of that object once the scope is left.
        /// </summary>
        [Fact]
        public void UsingStatement()
        {
            var resource = new Resource();

            using (new DisposingClass(resource))
            {
                Assert.False(resource.IsDisposed);
            }

            Assert.True(resource.IsDisposed);
        }
    }
}
