using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Alias used in a test
using StringToString = System.Collections.Generic.Dictionary<string, string>;

namespace csharp
{
    // Extension methods cannot be nested with a class.
    // Define here first and using immediately to aid understanding.
    public class Post
    {
        public DateTime PostedOn;
    }

    public static class PostFilters
    {
        public static IEnumerable<Post> PostedAfter
            (this IEnumerable<Post> posts, DateTime dateTime)
        {
            return posts.Where(post => post.PostedOn > dateTime);
        }
    }

    /// <summary>
    /// Run me with Test => Run => All Tests
    /// </summary>
    [TestClass]
    public class Main
    {
        public Expression<Func<Post, bool>> PostedAfter(DateTime cutoffDate)
        {
            return post => post.PostedOn > cutoffDate;
        }

        /// <summary>
        /// Demonstrating how collections can be queried with LINQ via an
        /// a lambda, extension method and expression tree; the choice is yours!
        /// </summary>
        [TestMethod]
        public void TestLinqFiltering()
        {
            var latestDate = new DateTime(2010, 6, 15);

            var posts = new List<Post>
            {
                { new Post { PostedOn = new DateTime(1990, 12, 31) } },
                { new Post { PostedOn = new DateTime(2000, 1, 1) } },
                { new Post { PostedOn = latestDate } }
            };

            var cutoff = new DateTime(2000, 1, 1);

            var standardQuery = posts.Where(p => p.PostedOn > cutoff);
            var extensionMethod = posts.PostedAfter(cutoff);

            var expressionTree =
                posts.AsQueryable().Where(PostedAfter(cutoff));

            Assert.AreEqual(standardQuery.Count(), 1);
            Assert.AreEqual(extensionMethod.Count(), 1);
            Assert.AreEqual(expressionTree.Count(), 1);

            Assert.AreEqual(standardQuery.First().PostedOn, latestDate);
            Assert.AreEqual(extensionMethod.First().PostedOn, latestDate);
            Assert.AreEqual(expressionTree.First().PostedOn, latestDate);
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

        class Resource
        {
            public bool IsDisposed { get; set; }

            public Resource()
            {
                IsDisposed = false;
            }
        }

        class DisposingClass : IDisposable
        {
            protected Resource _resource;

            public DisposingClass(Resource resource)
            {
                _resource = resource;
            }

            void IDisposable.Dispose()
            {
                _resource.IsDisposed = true;
            }
        }

        /// <summary>
        /// Demonstrates how the using statement defines a scope
        /// and automatically disposes of objects once that scope is left
        /// </summary>
        [TestMethod]
        public void TestUsingStatement()
        {
            var resouce = new Resource();

            using (var disposingClass = new DisposingClass(resouce))
            {
                Assert.IsFalse(resouce.IsDisposed);
            }

            Assert.IsTrue(resouce.IsDisposed);
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
                var willThrow = (EmptyToo)empty;
                Assert.Fail("Should throw!");
            }
            catch (InvalidCastException) { }

            var willBeNull = empty as EmptyToo;
            Assert.IsNull(willBeNull);

            // Visual Studio may warn you here!
            // "The given expression is never of the provided (<type>) type"
            Assert.IsFalse(new Empty() is EmptyToo);
        }

        T getDefaultValue<T>()
        {
            return default(T);
        }

        class Blank { }

        /// <summary>
        /// Shows how the default value of generic works as expected
        /// with value (including nullable) and reference types
        /// </summary>
        [TestMethod]
        public void TestDefaultValue()
        {
            Assert.IsFalse(getDefaultValue<bool>());
            Assert.AreEqual(getDefaultValue<int>(), 0);
            Assert.IsNull(getDefaultValue<double?>());
            Assert.IsNull(getDefaultValue<List<string>>());
            Assert.IsNull(getDefaultValue<Blank>());
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

        class @class
        {
            public const string foo = "bar";
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
            string @string = "foo bar";
            Assert.AreEqual(@string, "foo bar");

            int nonKeyword = 6;
            Assert.AreEqual(@nonKeyword, 6);

            Assert.AreEqual(@class.foo, "bar");
        }

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
        /// A demonstration of how structs are great for simple objects,
        /// and can be more lightweight than class alternatives.
        /// Note how a constructor is needed, but isn't for the class,
        /// although does not need to be constructed with "new",
        /// as it is a value type.
        /// </summary>
        public void TestStruct()
        {
            CoordinatesValueType coordinatesStack;
            coordinatesStack.x = 5;
            coordinatesStack.y = 10;

            var coordinatesHeap = new CoordinatesReferenceType {
                x = 5,
                y = 10
            };
            
            Assert.IsTrue(Marshal.SizeOf(coordinatesStack)
                < Marshal.SizeOf(coordinatesHeap));
        }

        [StructLayout(LayoutKind.Explicit)]
        public class RGB
        {
            [FieldOffset(2)]
            public byte R;

            [FieldOffset(1)]
            public byte G;

            [FieldOffset(0)]
            public byte B;

            /// <summary>
            /// This spans the other three fields,
            /// but isn't useful to consumers unless converted via AsHex
            /// </summary>
            [FieldOffset(0)]
            private int _Int32;

            public RGB() { }

            public RGB(byte R, byte G, byte B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }

            public string AsHex()
            {
                return _Int32.ToString("X6");
            }

            public RGB FromHex(string hex)
            {
                _Int32 = int.Parse
                    (hex, System.Globalization.NumberStyles.HexNumber);

                return this;
            }
        }

        /// <summary>
        /// Emulate C++ unions - shared memory space - and showing a simple use
        /// case that hides the overall integer representation of the individual
        /// bytes of an RGB value, but provides a method that converts that
        /// value to the more useful hex code.
        /// Also will convert a hex code, for completeness.
        /// </summary>
        /// <remarks>Compare this to the equivalent C++ test!</remarks>
        [TestMethod]
        public void TestUnion()
        {
            // Byte is a value type, and will default to 0,
            // so the default colour will be black!
            var black = new RGB();
            Assert.AreEqual(black.AsHex(), "000000");

            var coral = new RGB(255, 125, 125);
            Assert.AreEqual(coral.AsHex(), "FF7D7D");

            var coralConverted = new RGB().FromHex("FF7D7D");
            Assert.AreEqual(coralConverted.R, 255);
            Assert.AreEqual(coralConverted.G, 125);
            Assert.AreEqual(coralConverted.B, 125);
        }

        [DebuggerDisplay("{Name} from {Town}")]
        class EasyDebugPerson
        {
            public string Name { get; private set; }
            public string Town { get; private set; }

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
            var debugMe = new EasyDebugPerson("Dave", "Essex");
            Debugger.Break();
        }

        /// <summary>
        /// Simple but useful ability to use the current index of the
        /// enumerated collection when projecting it.
        /// </summary>
        [TestMethod]
        public void TestProjectionWithIndex()
        {
            var strings = new List<string> { "foo", "bar" };

            var numberedStrings = strings.Select
                ((s, i) => String.Format("{0}: {1}", s, i)).ToArray();

            Assert.AreEqual(numberedStrings.Length, 2);
            Assert.AreEqual(numberedStrings[0], "foo: 0");
            Assert.AreEqual(numberedStrings[1], "bar: 1");
        }
    }
}
