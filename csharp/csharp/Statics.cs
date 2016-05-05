using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    class TestClass
    {
        public static int A = 5;
        public static int B = A;

        public static int C = D;
        public static int D = 7;

        private TestClass() { }
    }

    [TestClass]
    public class Statics
    {
        [TestMethod]
        public void BewareDeclarationOrder()
        {
            Assert.AreEqual(TestClass.A, TestClass.B);

            Assert.AreNotEqual(TestClass.C, TestClass.D,
                "D was not assigned when C used it!");
        }
    }
}
