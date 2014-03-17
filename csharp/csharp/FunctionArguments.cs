using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// A quick comparison of how the ref and out keywords affect function
    /// parameter passing, and how they also make the distinction explicit
    /// by requiring the caller to use the keywords!
    /// </summary>
    [TestClass]
    public class FunctionArguments
    {
        void modifyByRef(ref string name)
        {
            name = "Foo Bar";
        }

        /// <summary>
        /// Shows a value type being passed as a reference parameter to a 
        /// function, which requires it to be initialised first
        /// </summary>
        [TestMethod]
        public void TestRefKeywordValueType()
        {
            string modifyMe = "Mister E";
            modifyByRef(ref modifyMe);
            Assert.AreEqual(modifyMe, "Foo Bar");
        }

        void modifyByOut(out string name)
        {
            name = "Foo Bar";
        }

        /// <summary>
        /// Shows a value type being passed as an out parameter to a function,
        /// which does not need to be initialised
        /// </summary>
        [TestMethod]
        public void TestOutKeywordValueType()
        {
            string modifyMe;
            modifyByOut(out modifyMe);
            Assert.AreEqual(modifyMe, "Foo Bar");
        }

        class Test
        {
            public bool modified;
        }

        void modifyObject(Test test)
        {
            test.modified = true;
        }

        void replaceObject(Test test)
        {
            test = new Test();
        }

        /// <summary>
        /// Demonstration of how, although reference types are always passed
        /// by reference, they can be modified but not replaced
        /// </summary>
        [TestMethod]
        public void TestRefTypesCopyReferencePointer()
        {
            var test = new Test();
            modifyObject(test);
            Assert.IsTrue(test.modified);

            replaceObject(test);
            Assert.IsTrue(test.modified);
        }

        void modifyObjectViaActualReference(ref Test test)
        {
            test.modified = true;
        }

        void reallyReplaceObject(ref Test test)
        {
            test = new Test();
        }

        /// <summary>
        /// SHowing how, when the ref keyword is used, references types
        /// can be replaced when passed as a parameter to a function
        /// </summary>
        [TestMethod]
        public void TestRefTypesUseActualPointer()
        {
            var test = new Test();
            modifyObjectViaActualReference(ref test);
            Assert.IsTrue(test.modified);

            reallyReplaceObject(ref test);
            Assert.IsFalse(test.modified);
        }
    }
}
