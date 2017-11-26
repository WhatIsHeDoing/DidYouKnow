using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace csharp
{
    // ReSharper disable ClassNeverInstantiated.Local
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// This class houses unit tests that have not yet been grouped.
    /// </summary>
    public class Ungrouped
    {
        /// <summary>
        /// Demonstrates chaining the null-coalescing operator.
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/ms173224.aspx
        /// </remarks>
        [Fact]
        public void ChainingNullCoalescingOperator()
        {
            int? firstNull = null;
            int? secondNull = null;
            
            // ReSharper disable ConstantNullCoalescingCondition
            Assert.Equal(firstNull ?? secondNull ?? 123, 123);
        }

        static string RegisterMethod<T>(T method, string name) where T : class =>
            method != null ? name : "";

        static string RegisterMethod<T>(Expression<Action<T>> action) where T : class
        {
            var expression = (action.Body as MethodCallExpression);
            return (expression != null) ? expression.Method.Name : "";
        }

        class UseMe
        {
            [SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic")]
            public void SomeMethod() { }
        }

        /// <summary>
        /// Using strongly-typed method registration,
        /// thanks to expression trees.
        /// </summary>
        [Fact]
        public void StronglyTypedMethodRegistration()
        {
            Assert.Equal(RegisterMethod(typeof(UseMe), "SomeMethod"),
                RegisterMethod<UseMe>(c => c.SomeMethod()));
        }

        class Empty { }
        
        [SuppressMessage("Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses")]
        class EmptyToo { }

        /// <summary>
        /// Differentiates standard casting, "as" casting and the "is" check.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1800:DoNotCastUnnecessarily")]
        [SuppressMessage("Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "willThrow")]
        [SuppressMessage("Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "csharp.Ungrouped+Empty")]
        [Fact]
        public void CastVersusAsVersusIs()
        {
            object empty = new Empty();

            Assert.Throws<InvalidCastException>(() => (EmptyToo)empty);

            // ReSharper disable once ExpressionIsAlwaysNull
            var willBeNull = empty as EmptyToo;
            Assert.Null(willBeNull);

#pragma warning disable CS0184
            // Visual Studio may warn you here!
            // "The given expression is never of the provided (<type>) type"
            // ReSharper disable once IsExpressionAlwaysFalse
            Assert.False(new Empty() is EmptyToo);
#pragma warning restore CS0184
        }

        [DebuggerDisplay("{Name} from {Town}")]
        class EasyDebugPerson
        {
            public readonly string Name;
            public readonly string Town;

            public EasyDebugPerson(string name, string town)
            {
                Name = name;
                Town = town;
            }
        }

        /// <summary>
        /// Nothing to see here, except if you run Test => Debug
        /// and notice the value of debugMe is '"Dave" from "Essex"'.
        /// </summary>
        [Fact]
        public void DebuggerDisplay()
        {
            var debugMe = new EasyDebugPerson("Dave", "Essex");
            Assert.Equal("Dave", debugMe.Name);
            Assert.Equal("Essex", debugMe.Town);
            Debugger.Break();
        }
    }
}
