using Xunit;

namespace csharp
{
    /// <summary>
    /// Variables can vary in more ways than you may assume.
    /// </summary>
    public class Variables
    {
        /// <summary>
        /// Shows how Unicode characters can be used in variable names.
        /// Hover over to see the variable used in the assertion!
        /// This is unreadable, but prevents the need for Unicode encoding.
        /// </summary>
        [Fact]
        public void VariableNamesCanContainUnicodeCharacters()
        {
            const string ὧὃḁḣ = "woah";
            Assert.NotNull(\u1f67\u1f43\u1e01\u1e23);
        }

#pragma warning disable IDE1006 // Naming Styles
        class @class
        {
            private @class() { }
            public const string Foo = "bar";
        }
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Avoid keyword name clashes with the "@" symbol.
        /// You can also use it when using existing definitions.
        /// Used to enable interoperability between CLI languages,
        /// but mind your colleagues if you decide to use it for other reasons!
        /// </summary>
        [Fact]
        public void KeywordVariableNames()
        {
            const string @string = "foo bar";
            Assert.Equal("foo bar", @string);

            const int nonKeyword = 6;
            Assert.Equal(6, @nonKeyword);

            Assert.Equal("bar", @class.Foo);
        }

        class Person
        {
            public int Age { get; set; }
            public required string Name { get; set; }
        }

        /// <summary>
        /// Shows how an anonymous type is created
        /// compared to a class instance instantiated with an initializer.
        /// </summary>
        [Fact]
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

            Assert.Equal(anonymousPerson.Age, person.Age);
            Assert.Equal(anonymousPerson.Name, person.Name);
        }
    }
}
