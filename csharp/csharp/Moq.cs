using System;
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
        public interface IFetchData
        {
            int ID { get; }
            string Fetch();
        }

        /// <summary>
        /// A simple test to show that stubs are of the type that they mock
        /// </summary>
        [TestMethod]
        public void TestStubImplementsType()
        {
            Assert.IsTrue
                (new Mock<IFetchData>().Object is IFetchData);
        }

        /// <summary>
        /// Mocking a readonly attribute
        /// </summary>
        [TestMethod]
        public void TestStubReadonlyType()
        {
            var fetchData = new Mock<IFetchData>();
            fetchData.SetupGet(f => f.ID).Returns(123);
            Assert.AreEqual(fetchData.Object.ID, 123);
        }

        public class DataFetcher
        {
            public IFetchData fetchData { get; set; }

            public string GetData()
            {
                var data = fetchData.Fetch();

                if (String.IsNullOrEmpty(data)) {
                    throw new ArgumentException("No data returned!");
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
        public void TestStubFunction()
        {
            var fetchData = new Mock<IFetchData>();
            fetchData
                .Setup(f => f.Fetch())
                .Returns("Hi!");

            Assert.AreEqual(new DataFetcher
            {
                fetchData = fetchData.Object
            }.GetData(), "Hi!");

            var fetchDataFail = new Mock<IFetchData>();

            fetchDataFail
                .Setup(f => f.Fetch())
                .Throws(new ArgumentNullException());

            try
            {
                new DataFetcher
                {
                    fetchData = fetchDataFail.Object
                }.GetData();
            }
            catch (ArgumentNullException e)
            {
                return;
            }

            Assert.Fail("exception not thrown!");
        }
    }
}
