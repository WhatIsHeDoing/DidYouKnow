using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace csharp
{
    /// <summary>
    /// Pity thee!  Err, demonstrating mocking via a third-party library,
    /// Rhino Mocks, installed via NuGet
    /// </summary>
    [TestClass]
    public class RhinoMocks
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
                (MockRepository.GenerateStub<IFetchData>() is IFetchData);
        }

        /// <summary>
        /// Mocking a readonly attribute
        /// </summary>
        [TestMethod]
        public void TestStubReadonlyType()
        {
            var fetchData = MockRepository.GenerateStub<IFetchData>();
            fetchData.Stub(f => f.ID).Return(123);
            Assert.AreEqual(fetchData.ID, 123);
        }

        public class DataFetcher
        {
            public IFetchData fetchData { get; set; }

            public string GetData()
            {
                var data = fetchData.Fetch();

                if (String.IsNullOrEmpty(data)) {
                    throw new ApplicationException("no data returned!");
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
            var fetchData = MockRepository.GenerateStub<IFetchData>();
            fetchData.Stub<IFetchData, string>(f => f.Fetch()).Return("Hi!");

            Assert.AreEqual(new DataFetcher
            {
                fetchData = fetchData
            }.GetData(), "Hi!");

            var fetchDataFail = MockRepository.GenerateStub<IFetchData>();
            fetchDataFail.Stub<IFetchData, string>(f => f.Fetch()).Return("");

            try
            {
                new DataFetcher
                {
                    fetchData = fetchDataFail
                }.GetData();
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual(e.Message, "no data returned!");
                return;
            }

            Assert.Fail("exception not thrown!");
        }
    }
}
