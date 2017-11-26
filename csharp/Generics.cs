using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

// Alias used in a test.
using StringToString = System.Collections.Generic.Dictionary<string, string>;

namespace csharp
{
    /// <summary>
    /// Some interesting uses and features of generics.
    /// </summary>
    public class Generics
    {
        /// <summary>
        /// Shows how an alias to a generic can be used
        /// to potentially simplify the use of complex structures.
        /// </summary>
        [Fact]
        public void AliasedGeneric()
        {
            var aliasedMap = new StringToString
            {
                { "Darren", "Hickling" }
            };

            var map = new Dictionary<string, string>
            {
                { "Darren", "Hickling" }
            };

            Assert.Equal(aliasedMap["Darren"], map["Darren"]);
        }

        static T ReturnValue<T>(T value) => value;

        /// <summary>
        /// Why explicitly state the type,
        /// when the compiler can determine it for you?
        /// </summary>
        [Fact]
        public void ImplicitGenerics() =>
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            Assert.Equal(ReturnValue(23), ReturnValue(23));

        static T GetDefaultValue<T>() => default(T);

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses")]
        class Blank { }

        /// <summary>
        /// Shows how the default value of generic works as expected
        /// with value (including nullable) and reference types.
        /// </summary>
        [Fact]
        public void DefaultValue()
        {
            Assert.False(GetDefaultValue<bool>());
            Assert.Equal(0, GetDefaultValue<int>());
            Assert.Null(GetDefaultValue<double?>());
            Assert.Null(GetDefaultValue<List<string>>());
            Assert.Null(GetDefaultValue<Blank>());
        }

        T[] SliceFromStart<T>(Span<T> source, int length)
            => source.Slice(0, length).ToArray();

        /// <summary>
        /// Use a data collection without copying or assignment thanks to Span.
        /// </summary>
        [Fact]
        public void Span()
        {
            var test = new[] { 1, 2, 3, 4 };
            var actual = SliceFromStart(test.AsSpan(), 3);
            Assert.Equal(new[] { 1, 2, 3 }, actual);
        }
    }
}
