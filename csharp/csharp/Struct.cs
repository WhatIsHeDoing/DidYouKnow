using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// A demonstration of how structs are great for simple objects,
    /// and can be more lightweight than class alternatives.
    /// </summary>
    [TestClass]
    public class Struct
    {
        struct CoordinatesValueType
        {
            public int X;
            public int Y;
        }

        class CoordinatesReferenceType
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// Compares the use of a struct versus a class containing
        /// identical fields.  Note that the struct does not need to be
        /// constructed with "new", as it is a value type.
        /// </summary>
        [TestMethod]
        public void StructTest()
        {
            const int x = 5;
            const int y = 5;

            CoordinatesValueType coordinatesStack;
            coordinatesStack.X = x;
            coordinatesStack.Y = y;

            var coordinatesHeap = new CoordinatesReferenceType
            {
                X = x,
                Y = y
            };

            Assert.AreEqual(coordinatesStack.X, coordinatesHeap.X);
            Assert.AreEqual(coordinatesStack.Y, coordinatesHeap.Y);
        }
    }
}
