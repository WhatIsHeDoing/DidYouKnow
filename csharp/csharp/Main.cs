using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    // ReSharper disable ClassNeverInstantiated.Local
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// This main test class is used to house unit tests
    /// that haven't yet been grouped under test classes
    /// </summary>
    [TestClass]
    public class Main
    {
        /// <summary>
        /// Shows that Unicode characters can be used in variables names.
        /// Hover over to read the real name!
        /// This is unreadable, but prevents the file needing Unicode encoding.
        /// </summary>
        [TestMethod]
        public void TestCanUseUnicodeCharactersInVariableNames()
        {
            const string ὧὃḁḣ = "woah";
            Assert.IsNotNull(\u1f67\u1f43\u1e01\u1e23);
        }

        /// <summary>
        /// Demonstrates chaining the null-coalescing operator
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/ms173224.aspx
        /// </remarks>
        [TestMethod]
        public void TestChainingNullCoalescingOperator()
        {
            int? firstNull = null;
            int? secondNull = null;
            
            // ReSharper disable ConstantNullCoalescingCondition
            Assert.AreEqual(firstNull ?? secondNull ?? 123, 123);
        }

        class Person
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Shows how an anonymous type is created
        /// compared to a class instance instantiated with an initialiser
        /// </summary>
        [TestMethod]
        public void TestAnonymousTypeVerusObjectInitialiser()
        {
            var anonymousPerson = new
            {
                Age = 31,
                Name = "Darren"
            };

            var person = new Person
            {
                Age = 31,
                Name = "Darren"
            };

            Assert.AreEqual(anonymousPerson.Age, person.Age);
            Assert.AreEqual(anonymousPerson.Name, person.Name);
        }

        /// <summary>
        /// Ignoring escape patterns and respecting formatting
        /// thanks to verbatim strings
        /// </summary>
        [TestMethod]
        public void TestVerbatimString()
        {
            const string verbatim = @"Hello\n
    World";

            const string standard = "Hello\\n\r\n    World";
            Assert.AreEqual(verbatim, standard);
        }

        string RegisterMethod<T>(T method, string name) where T : class
        {
            return (method != null) ? name : "";
        }

        string RegisterMethod<T>(Expression<Action<T>> action) where T : class
        {
            var expression = (action.Body as MethodCallExpression);
            return (expression != null) ? expression.Method.Name : "";
        }

        public class MyClass
        {
            public void SomeMethod() { }
        }

        /// <summary>
        /// Using strongly-typed method registration, thanks to expression trees
        /// </summary>
        [TestMethod]
        public void TestStronglyTypedMethodRegistration()
        {
            Assert.AreEqual(RegisterMethod(typeof(MyClass), "SomeMethod"),
                RegisterMethod<MyClass>(c => c.SomeMethod()));
        }

        class Empty { }
        class EmptyToo { }

        /// <summary>
        /// Differentiates standard casting, "as" casting and the "is" check
        /// </summary>
        [TestMethod]
        public void TestCastVersusAsVersusIs()
        {
            Object empty = new Empty();

            try
            {
                // ReSharper disable once UnusedVariable
                // ReSharper disable once PossibleInvalidCastException
                var willThrow = (EmptyToo)empty;
                Assert.Fail("Should throw!");
            }
            catch (InvalidCastException) { }

            // ReSharper disable once ExpressionIsAlwaysNull
            var willBeNull = empty as EmptyToo;
            Assert.IsNull(willBeNull);

            // Visual Studio may warn you here!
            // "The given expression is never of the provided (<type>) type"
            Assert.IsFalse(new Empty() is EmptyToo);
        }

        class @class
        {
            public const string Foo = "bar";
        }

        /// <summary>
        /// Avoid keyword name clashes with the "@" symbol.
        /// You can also use it when using existing definitions.
        /// Used to enable interoperability between CLI languages,
        /// but mind your colleagues if you decide to use it for other reasons!
        /// </summary>
        [TestMethod]
        public void TestKeywordVariableNames()
        {
            const string @string = "foo bar";
            Assert.AreEqual(@string, "foo bar");

            const int nonKeyword = 6;
            Assert.AreEqual(@nonKeyword, 6);

            Assert.AreEqual(@class.Foo, "bar");
        }

        [DebuggerDisplay("{Name} from {Town}")]
        class EasyDebugPerson
        {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
            public string Town { get; set; }

            public EasyDebugPerson(string name, string town)
            {
                Name = name;
                Town = town;
            }
        }

        /// <summary>
        /// Nothing to see here, except if you run Test => Debug and notice
        /// the value of debugMe is '"Dave" from "Essex"'!
        /// </summary>
        [TestMethod]
        public void TestDebuggerDisplay()
        {
            // ReSharper disable once UnusedVariable
            var debugMe = new EasyDebugPerson("Dave", "Essex");
            Debugger.Break();
        }
    }
}
