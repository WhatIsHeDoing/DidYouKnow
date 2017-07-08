using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    [TestClass]
    public class Linq
    {
        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<Post, bool>> PostedAfter
            (DateTime cutoffDate) =>
                post => post.PostedOn > cutoffDate;

        /// <summary>
        /// Demonstrating how collections can be queried with LINQ via an
        /// a lambda, extension method and expression tree; the choice is yours!
        /// </summary>
        [TestMethod]
        public void LinqFiltering()
        {
            var latestDate = new DateTime(2010, 6, 15);

            var posts = new List<Post>
            {
                new Post { PostedOn = new DateTime(1990, 12, 31) },
                new Post { PostedOn = new DateTime(2000, 1, 1) },
                new Post { PostedOn = latestDate }
            };

            var cutoff = new DateTime(2000, 1, 1);

            var standardQuery = posts.Where(p => p.PostedOn > cutoff).ToList();
            var extensionMethod = posts.PostedAfter(cutoff).ToList();

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
        /// Simple but useful ability to use the current index of the
        /// enumerated collection when projecting it.
        /// </summary>
        [TestMethod]
        public void ProjectionWithIndex()
        {
            var strings = new List<string> { "foo", "bar" };

            var numberedStrings = strings.Select
                ((s, i) => String.Format(CultureInfo.CurrentCulture,
                    "{0}: {1}", s, i)).ToArray();

            Assert.AreEqual(numberedStrings.Length, 2);
            Assert.AreEqual(numberedStrings[0], "foo: 0");
            Assert.AreEqual(numberedStrings[1], "bar: 1");
        }
    }
}
