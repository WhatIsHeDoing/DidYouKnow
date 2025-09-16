using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace csharp
{
    // Extension methods cannot be nested within a class.
    public class Post
    {
        public DateTime PostedOn { get; set; }
    }

    public static class PostFilters
    {
        public static IEnumerable<Post> PostedAfter
            (this IEnumerable<Post> posts, DateTime dateTime) =>
                posts.Where(post => post.PostedOn > dateTime);
    }

    /// <summary>
    /// LINQ is utterly amazing, so let these examples inspire you!
    /// </summary>
    public class Linq
    {
        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<Post, bool>> PostedAfter
            (DateTime cutOffDate) =>
                post => post.PostedOn > cutOffDate;

        /// <summary>
        /// Demonstrating how collections can be queried with LINQ via an
        /// a lambda, extension method and expression tree; the choice is yours!
        /// </summary>
        [Fact]
        public void LinqFiltering()
        {
            var latestDate = new DateTime(2010, 6, 15);

            var posts = new List<Post>
            {
                new() { PostedOn = new DateTime(1990, 12, 31) },
                new() { PostedOn = new DateTime(2000, 1, 1) },
                new() { PostedOn = latestDate }
            };

            var cutOff = new DateTime(2000, 1, 1);

            var standardQuery = posts.Where(p => p.PostedOn > cutOff).ToList();
            var extensionMethod = posts.PostedAfter(cutOff).ToList();

            var expressionTree = posts.AsQueryable().Where(PostedAfter(cutOff));

            Assert.Single(standardQuery);
            Assert.Single(extensionMethod);
            Assert.Equal(1, expressionTree.Count());

            Assert.Equal(standardQuery.First().PostedOn, latestDate);
            Assert.Equal(extensionMethod.First().PostedOn, latestDate);
            Assert.Equal(expressionTree.First().PostedOn, latestDate);
        }

        /// <summary>
        /// Simple but useful ability to use the current index of the
        /// enumerated collection when projecting it.
        /// </summary>
        [Fact]
        public void ProjectionWithIndex()
        {
            var strings = new List<string> { "foo", "bar" };

            var numberedStrings = strings.Select
                ((s, i) => string.Format(CultureInfo.CurrentCulture,
                    "{0}: {1}", s, i)).ToArray();

            Assert.Equal(2, numberedStrings.Length);
            Assert.Equal("foo: 0", numberedStrings[0]);
            Assert.Equal("bar: 1", numberedStrings[1]);
        }
    }
}
