namespace csharp
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Xunit;

    /// <summary>
    /// Fun with strings.
    /// </summary>
    public class Strings
    {
        /// <summary>
        /// Ignoring escape patterns and respecting formatting, thanks to
        /// verbatim strings. Note that a whitespace-replaced version is
        /// tested, as the line formatting of this file can vary.
        /// Use <code>System.Environment.NewLine</code> for consistency!
        /// </summary>
        [Fact]
        public void VerbatimString()
        {
            const string verbatim = @"Hello
    World";

            var actual = Regex.Replace(verbatim, @"\s", "");
            const string expected = "HelloWorld";
            Assert.Equal(expected, actual);
        }

        class TestClass
        {
            public static string StaticString = "remembered";
            public const string ConstString = "remembered";
            public string InstanceString = "remembered";
        }

        /// <summary>
        /// Demonstrating that strings from all scopes are interned.
        /// </summary>
        [Fact]
        public void StringInterningByDefault()
        {
            var test = new TestClass();
            var internedStaticString = string.IsInterned(TestClass.StaticString);
            var internedConstString = string.IsInterned(TestClass.ConstString);
            var internedInstanceString = string.IsInterned(test.InstanceString);

            Assert.True(ReferenceEquals(internedStaticString, "remembered"));
            Assert.True(ReferenceEquals(internedStaticString, internedConstString));
            Assert.True(ReferenceEquals(internedStaticString, internedInstanceString));
            Assert.True(ReferenceEquals(internedConstString, internedInstanceString));

            Assert.False(ReferenceEquals(internedStaticString, "different!"));
        }

        /// <summary>
        /// Compares the different options available to compose strings.
        /// String concatentation is less performant than other techniques,
        /// but it does not fair as badly as you think.
        /// </summary>
        [Fact]
        public void StringCompositionComparison()
        {
            // First, declare our little sentence structure.
            // Note that these strings are deliberately not const.
            string hello = "Hello";
            string space = " ";
            string @this = "this";
            string @is = "is";
            string a = "a";
            string sentence = "sentence";
            string expected = "Hello this is a sentence";

            const uint repeatTimes = uint.MaxValue;

            // Set out repeat and time local function.
            TimeSpan RepeatAndTime(uint times, Func<string> toTime)
            {
                var timer = Stopwatch.StartNew();
                _ = toTime();
                timer.Stop();
                return timer.Elapsed;
            }

            // First up: concatenation. Immutable strings
            // create more immutable strings, consuming memory and time.
            Func<string> concatenation = () =>
                hello +
                space +
                @this +
                space +
                @is +
                space +
                a +
                space +
                sentence;

            var concatenationTime = RepeatAndTime(repeatTimes, concatenation);

            // Next up: StringBuilder, from which only calling
            // ToString will generate the string we want.
            Func<string> stringBuilder = () => new StringBuilder()
                .Append(hello)
                .Append(space)
                .Append(@this)
                .Append(space)
                .Append(@is)
                .Append(space)
                .Append(a)
                .Append(space)
                .Append(sentence)
                .ToString();

            var stringBuilderTime = RepeatAndTime(repeatTimes, stringBuilder);

            // Now to format. This was our best friend until...
            Func<string> format = () => string.Format(
                "{0}{1}{2}{1}{3}{1}{4}{1}{5}",
                hello, space, @this, @is, a, sentence);

            var formatTime = RepeatAndTime(repeatTimes, format);

            // string interning, which is far more readable.
            Func<string> intern = () =>
                $"{hello}{space}{@this}{space}{@is}{space}{a}{space}{sentence}";

            var internTime = RepeatAndTime(repeatTimes, intern);

            // Ensure all generated strings are correct!
            Assert.Equal(expected, concatenation());
            Assert.Equal(expected, stringBuilder());
            Assert.Equal(expected, format());
            Assert.Equal(expected, intern());

            // Finally: ensure string formatting interning
            // are faster than conatentation and string builder.
            Assert.True(formatTime < concatenationTime);
            Assert.True(formatTime < stringBuilderTime);
            Assert.True(internTime < concatenationTime);
            Assert.True(internTime < stringBuilderTime);

            // Is string interning _that_ much faster?
            // Well, actually, no...
            Assert.True((internTime.Ticks * 5) > concatenationTime.Ticks);

            // Bonus: check the output section of the
            // passed unit test to see the difference in the timings.
            var shortestTicks = Convert.ToDouble(new[]
            {
                concatenationTime,
                stringBuilderTime,
                formatTime,
                internTime
            }.Min().Ticks);

            double Diff(TimeSpan time) => (time.Ticks / shortestTicks) - 1;

            Console.WriteLine(
$@"
{"Concatenation",-20} {concatenationTime} [+{Diff(concatenationTime):0.##%}]
{"StringBuilder",-20} {stringBuilderTime} [+{Diff(stringBuilderTime):0.##%}]
{"Format", -20} {formatTime} [+{Diff(formatTime):0.##%}]
{"Intern", -20} {internTime} [+{Diff(internTime):0.##%}]
");
        }
    }
}
