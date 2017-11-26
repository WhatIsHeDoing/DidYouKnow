using Xunit;

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

    /// <summary>
    /// Beware statics!
    /// </summary>
    public class Statics
    {
        /// <summary>
        /// Shows why the declaration order of static variables is important!
        /// </summary>
        [Fact]
        public void BewareDeclarationOrder()
        {
            Assert.Equal(TestClass.A, TestClass.B);
            Assert.NotEqual(TestClass.C, TestClass.D);
        }
    }
}
