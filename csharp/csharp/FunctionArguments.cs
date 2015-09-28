using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    // ReSharper disable RedundantAssignment

    /// <summary>
    /// A demonstration of default and named parameters, and a comparison of how
    /// the ref and out keywords affect function parameter passing, and how they
    /// also make the distinction explicit by requiring
    /// the caller to use the keywords!
    /// </summary>
    [TestClass]
    public class FunctionArguments
    {
        static string DefaultParameter(string name, int age=18)
        {
            return string.Format(CultureInfo.CurrentCulture,
                "{0} is {1} years old", name, age);
        }

        /// <summary>
        /// Showing how a function with a default parameter can be called
        /// value set
        /// </summary>
        [TestMethod]
        public void DefaultParameters()
        {
            Assert.AreEqual(DefaultParameter("Darren"),
                "Darren is 18 years old");

            Assert.AreEqual(DefaultParameter("Darren", 31),
                "Darren is 31 years old");
        }

        static string BoolArgument(bool test)
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
        public void NamedParameters()
        {
            Assert.AreEqual(BoolArgument(true), "foo");
            Assert.AreEqual(BoolArgument(test: false), "bar");
        }

        static void ModifyByRef(ref string name)
        {
            name = "Foo Bar";
        }

        /// <summary>
        /// Shows a value type being passed as a reference parameter to a 
        /// function, which requires it to be initialised first
        /// </summary>
        [TestMethod]
        public void RefKeywordValueType()
        {
            var modifyMe = "Mister E";
            ModifyByRef(ref modifyMe);
            Assert.AreEqual(modifyMe, "Foo Bar");
        }

        static void ModifyByOut(out string name)
        {
            name = "Foo Bar";
        }

        /// <summary>
        /// Shows a value type being passed as an out parameter to a function,
        /// which does not need to be initialised
        /// </summary>
        [TestMethod]
        public void OutKeywordValueType()
        {
            string modifyMe;
            ModifyByOut(out modifyMe);
            Assert.AreEqual(modifyMe, "Foo Bar");
        }

        class Test
        {
            public bool Modified;
        }

        static void ModifyObject(Test test)
        {
            test.Modified = true;
        }

        // ReSharper disable once UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage",
            "CA1801:ReviewUnusedParameters", MessageId = "test")]
        static void ReplaceObject(Test test)
        {
            test = new Test();
        }

        /// <summary>
        /// Demonstration of how, although reference types are always passed
        /// by reference, they can be modified but not replaced
        /// </summary>
        [TestMethod]
        public void RefTypesCopyReferencePointer()
        {
            var test = new Test();
            ModifyObject(test);
            Assert.IsTrue(test.Modified);

            ReplaceObject(test);
            Assert.IsTrue(test.Modified);
        }

        static void ModifyObjectViaActualReference(ref Test test)
        {
            test.Modified = true;
        }

        static void ReallyReplaceObject(ref Test test)
        {
            test = new Test();
        }

        /// <summary>
        /// SHowing how, when the ref keyword is used, references types
        /// can be replaced when passed as a parameter to a function
        /// </summary>
        [TestMethod]
        public void RefTypesUseActualPointer()
        {
            var test = new Test();
            ModifyObjectViaActualReference(ref test);
            Assert.IsTrue(test.Modified);

            ReallyReplaceObject(ref test);
            Assert.IsFalse(test.Modified);
        }

        static string ArgsToCsv(params object[] args)
        {
            return string.Join(",", args.Select(a => a.ToString()).ToList());
        }

        /// <summary>
        /// Supply a variable number of arguments to a method using "params"
        /// <see>System.Console.WriteLine</see>
        /// </summary>
        [TestMethod]
        public void VariableArgumentList()
        {
            Assert.AreEqual(ArgsToCsv(new object[] { "foo", 6 }), "foo,6");
        }
    }
}
