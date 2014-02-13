using System;
using System.Runtime.InteropServices;
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
            public int x, y;

            public CoordinatesValueType(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        class CoordinatesReferenceType
        {
            public int x, y;
        }

        /// <summary>
        /// Compares the use of a struct versus a class containing
        /// identical fields.  Note that the struct does not need to be
        /// constructed with "new", as it is a value type.
        /// </summary>
        [TestMethod]
        public void TestStruct()
        {
            const int x = 5;
            const int y = 5;

            CoordinatesValueType coordinatesStack;
            coordinatesStack.x = x;
            coordinatesStack.y = y;

            var coordinatesHeap = new CoordinatesReferenceType
            {
                x = x,
                y = y
            };

            Assert.AreEqual(coordinatesStack.x, coordinatesHeap.x);
            Assert.AreEqual(coordinatesStack.y, coordinatesHeap.y);
        }
    }
}
