﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace csharp
{
    /// <summary>
    /// Demonstrates how to use lazy instantiation.
    /// </summary>
    [TestClass]
    public class LazyInstantiation
    {
        class EpicConstruction
        {
            public readonly bool IsSetup;

            public EpicConstruction()
            {
                Thread.Sleep(1000);
                IsSetup = true;
            }
        }

        /// <summary>
        /// Shows how lazy instantiation delays the
        /// construction of an object until it is used.
        /// </summary>
        [TestMethod]
        public void LazyTest()
        {
            var start = DateTime.Now;

            var lazy = new Lazy<EpicConstruction>();
            Assert.IsFalse(lazy.IsValueCreated);

            var lazyTime = DateTime.Now - start;

            start = DateTime.Now;
            var lazyEpicConstruction = lazy.Value;
            var lazyAccessTime = DateTime.Now - start;

            Assert.IsTrue(lazy.IsValueCreated);
            Assert.IsTrue(lazyEpicConstruction.IsSetup);
            Assert.IsTrue(lazyTime < lazyAccessTime);

            start = DateTime.Now;
            var epicConstruction = new EpicConstruction();
            var expensiveEndTime = DateTime.Now - start;

            Assert.IsTrue(epicConstruction.IsSetup);
            Assert.IsTrue(lazyTime < expensiveEndTime);
        }
    }
}
