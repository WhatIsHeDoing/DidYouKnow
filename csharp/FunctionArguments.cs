using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Xunit;

namespace csharp
{
    // ReSharper disable RedundantAssignment

    /// <summary>
    /// A demonstration of default and named parameters, and a comparison of how
    /// the ref and out keywords affect function parameter passing, and how they
    /// also make the distinction explicit by requiring
    /// the caller to use the keywords!
    /// </summary>
    public class FunctionArguments
    {
        static string DefaultParameter(string name, int age = 18)
            => string.Format(CultureInfo.CurrentCulture,
                "{0} is {1} years old", name, age);

        /// <summary>
        /// Showing how a function with a default parameter can be called
        /// value set
        /// </summary>
        [Fact]
        public void DefaultParameters()
        {
            Assert.Equal("Darren is 18 years old", DefaultParameter("Darren"));
            Assert.Equal("Darren is 31 years old", DefaultParameter("Darren", 31));
        }

        static string BoolArgument(bool test) =>
            test ? "foo" : "bar";

        /// <summary>
        /// Comparison of calling a function with and without the parameter
        /// name being used. This is great when calling functions that
        /// take multiple parameters, one being a boolean, which
        /// often makes it unclear what that parameter is doing!
        /// </summary>
        [Fact]
        public void NamedParameters()
        {
            Assert.Equal("foo", BoolArgument(true));
            Assert.Equal("bar", BoolArgument(test: false));
        }

        static void ModifyByRef(ref string name) => name = "Foo Bar";

        /// <summary>
        /// Shows a value type being passed as a reference parameter to a 
        /// function, which requires it to be initialised first
        /// </summary>
        [Fact]
        public void RefKeywordValueType()
        {
            var modifyMe = "Mister E";
            ModifyByRef(ref modifyMe);
            Assert.Equal("Foo Bar", modifyMe);
        }

        static void ModifyByOut(out string name) =>
            name = "Foo Bar";

        /// <summary>
        /// Shows a value type being passed as an out parameter to a function,
        /// which does not need to be initialised
        /// </summary>
        [Fact]
        public void OutKeywordValueType()
        {
            ModifyByOut(out string modifyMe);
            Assert.Equal("Foo Bar", modifyMe);
        }

        class Test
        {
            public bool Modified;
        }

        static void ModifyObject(Test test) => test.Modified = true;

        // ReSharper disable once UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage",
            "CA1801:ReviewUnusedParameters", MessageId = "test")]
        static void ReplaceObject(Test test) =>
            test = new Test();

        /// <summary>
        /// Demonstration of how, although reference types are always passed
        /// by reference, they can be modified but not replaced
        /// </summary>
        [Fact]
        public void RefTypesCopyReferencePointer()
        {
            var test = new Test();
            ModifyObject(test);
            Assert.True(test.Modified);

            ReplaceObject(test);
            Assert.True(test.Modified);
        }

        static void ModifyObjectViaActualReference(ref Test test) =>
            test.Modified = true;

        static void ReallyReplaceObject(ref Test test) =>
            test = new Test();

        /// <summary>
        /// SHowing how, when the ref keyword is used, references types
        /// can be replaced when passed as a parameter to a function
        /// </summary>
        [Fact]
        public void RefTypesUseActualPointer()
        {
            var test = new Test();
            ModifyObjectViaActualReference(ref test);
            Assert.True(test.Modified);

            ReallyReplaceObject(ref test);
            Assert.False(test.Modified);
        }

        static string ArgsToCsv(params object[] args) =>
            string.Join(",", args.Select(a => a.ToString()).ToList());

        /// <summary>
        /// Supply a variable number of arguments to a method using "params"
        /// <see>System.Console.WriteLine</see>
        /// </summary>
        [Fact]
        public void VariableArgumentList() =>
            Assert.Equal("foo,6", ArgsToCsv(new object[] { "foo", 6 }));
    }
}
