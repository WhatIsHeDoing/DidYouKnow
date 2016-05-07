using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Variables can vary in more ways than you may assume.
    /// </summary>
    [TestClass]
    public class Variables
    {
        /// <summary>
        /// Shows how Unicode characters can be used in variable names.
        /// Hover over to see the variable used in the assertion!
        /// This is unreadable, but prevents the need for Unicode encoding.
        /// </summary>
        [TestMethod]
        public void VariableNamesCanContainUnicodeCharacters()
        {
            const string ὧὃḁḣ = "woah";
            Assert.IsNotNull(\u1f67\u1f43\u1e01\u1e23);
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
        public void KeywordVariableNames()
        {
            const string @string = "foo bar";
            Assert.AreEqual(@string, "foo bar");

            const int nonKeyword = 6;
            Assert.AreEqual(@nonKeyword, 6);

            Assert.AreEqual(@class.Foo, "bar");
        }

        class Person
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Shows how an anonymous type is created
        /// compared to a class instance instantiated with an initializer.
        /// </summary>
        [TestMethod]
        public void AnonymousTypeVersusObjectInitializer()
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
    }
}
