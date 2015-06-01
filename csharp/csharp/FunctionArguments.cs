using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// A demonstration of default and named parameters, and a comparison of how
    /// the ref and out keywords affect function parameter passing, and how they
    /// also make the distinction explicit by requiring
    /// the caller to use the keywords!
    /// </summary>
    [TestClass]
    public class FunctionArguments
    {
        string defaultParameter(string name, int age=18)
        {
            return String.Format("{0} is {1} years old", name, age);
        }

        /// <summary>
        /// Showing how a function with a default parameter can be called
        /// value set
        /// </summary>
        [TestMethod]
        public void TestDefaultParameters()
        {
            Assert.AreEqual(defaultParameter("Darren"),
                "Darren is 18 years old");

            Assert.AreEqual(defaultParameter("Darren", 31),
                "Darren is 31 years old");
        }

        string boolArgument(bool test)
        {
            return (test) ? "foo" : "bar";
        }

        /// <summary>
        /// Comparison of calling a function with and without the parameter
        /// name being used. This is great when calling functions that
        /// take multiple parameters, one being a boolean, which
        /// often makes it unclear what that parameter is doing!
        /// </summary>
        [TestMethod]
        public void TestNamedParameters()
        {
            Assert.AreEqual(boolArgument(true), "foo");
            Assert.AreEqual(boolArgument(test: false), "bar");
        }

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

        string argsToCSV(params object[] args)
        {
            return String.Join(",", args.Select(a => a.ToString()).ToList());
        }

        /// <summary>
        /// Supply a variable number of arguments to a method using "params"
        /// <see>System.Console.WriteLine</see>
        /// </summary>
        [TestMethod]
        public void TestVariableArgumentList()
        {
            Assert.AreEqual(argsToCSV(new object[] { "foo", 6 }), "foo,6");
        }
    }
}
