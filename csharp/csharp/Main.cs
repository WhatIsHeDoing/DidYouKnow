using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using StringToString = System.Collections.Generic.Dictionary<string, string>;

namespace csharp
{
    /// <summary>
    /// Run me with Test => Run => All Tests
    /// </summary>
    [TestClass]
    public class Main
    {
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

        IEnumerable<String> helloWorld()
        {
            yield return "Hello";
            yield return "World";
            yield break;
        }

        /// <summary>
        /// Generator methods via te yield keyword
        /// </summary>
        [TestMethod]
        public void TestYield()
        {
            var enumerator = helloWorld().GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "Hello");
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current, "World");
        }

        /// <summary>
        /// Shows how an alias to a generic can be used
        /// to potentially simplify the use of complex structures
        /// </summary>
        [TestMethod]
        public void TestAliasedGeneric()
        {
            StringToString aliasedMap = new StringToString()
            {
                { "Darren", "Hickling" }
            };

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "Darren", "Hickling" }
            };

            Assert.AreEqual(aliasedMap["Darren"], map["Darren"]);
        }

        T returnValue<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Why explicitly state the type,
        /// when the compiler can determine it for you?
        /// </summary>
        [TestMethod]
        public void TestImplicitGenerics()
        {
            Assert.AreEqual(returnValue<int>(23), returnValue(23));
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
    }
}
