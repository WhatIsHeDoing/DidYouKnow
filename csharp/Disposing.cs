using System;
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
            var resouce = new Resource();

            using (new DisposingClass(resouce))
            {
                Assert.False(resouce.IsDisposed);
            }

            Assert.True(resouce.IsDisposed);
        }
    }
}
