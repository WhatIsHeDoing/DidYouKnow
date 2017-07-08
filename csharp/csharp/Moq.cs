using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace csharp
{
    /// <summary>
    /// Pity thee!  Err, demonstrating mocking via a third-party library,
    /// Moq, installed via NuGet.
    /// </summary>
    [TestClass]
    public class Moq
    {
        [SuppressMessage("Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Required by Moq")]
        public interface IFetchData
        {
            int Id { get; }
            string Fetch();
        }

        /// <summary>
        /// A simple test to show that stubs are of the type that they mock
        /// </summary>
        [TestMethod]
        public void StubImplementsType() =>
            // ReSharper disable once IsExpressionAlwaysTrue
            Assert.IsTrue(new Mock<IFetchData>().Object is IFetchData);

        /// <summary>
        /// Mocking a readonly attribute
        /// </summary>
        [TestMethod]
        public void StubReadOnlyType()
        {
            var fetchData = new Mock<IFetchData>();
            fetchData.SetupGet(f => f.Id).Returns(123);
            Assert.AreEqual(fetchData.Object.Id, 123);
        }

        class DataFetcher
        {
            public IFetchData FetchData { private get; set; }

            public string GetData()
            {
                var data = FetchData.Fetch();

                if (string.IsNullOrEmpty(data)) {
                    throw new InvalidOperationException("No data returned!");
                }

                return data;
            }
        }

        /// <summary>
        /// Mocking a function by, again, just specifying the expected return
        /// value. This is really useful when you have no control over what
        /// will be returned from a function, like a data source, and wish to
        /// test the logic around that function call.
        /// </summary>
        [TestMethod]
        public void StubFunction()
        {
            var fetchData = new Mock<IFetchData>();

            fetchData
                .Setup(f => f.Fetch())
                .Returns("Hi!");

            Assert.AreEqual(new DataFetcher
            {
                FetchData = fetchData.Object
            }.GetData(), "Hi!");

            var fetchDataFail = new Mock<IFetchData>();

            fetchDataFail
                .Setup(f => f.Fetch())
                .Returns("");

            try
            {
                new DataFetcher
                {
                    FetchData = fetchDataFail.Object
                }.GetData();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual(e.Message, "No data returned!");
                return;
            }

            Assert.Fail("exception not thrown!");
        }
    }
}
