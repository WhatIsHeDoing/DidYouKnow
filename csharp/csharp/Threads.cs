using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

using AsyncDelegate = System.Func<double>;

namespace csharp
{
    [TestClass]
    public class Threads
    {
        public static double CalculationOne()
        {
            Thread.Sleep(500);
            return 1;
        }

        public static double CalculationTwo()
        {
            Thread.Sleep(1000);
            return 2;
        }

        public static double CalculationThree()
        {
            Thread.Sleep(1500);
            return 3;
        }

        [TestMethod]
        public void BeginInvokeTest()
        {
            // Synchronous.
            var start = DateTime.Now;

            var syncResult =
                CalculationOne() +
                CalculationTwo() +
                CalculationThree();

            var syncTime = DateTime.Now - start;

            // Begin invoke.
            AsyncDelegate calcOne = CalculationOne;
            AsyncDelegate calcTwo = CalculationTwo;
            AsyncDelegate calcThree = CalculationThree;

            start = DateTime.Now;

            var oneAsyncResult = calcOne.BeginInvoke(null, null);
            var twoAsyncResult = calcTwo.BeginInvoke(null, null);
            var threeAsyncResult = calcThree.BeginInvoke(null, null);

            var beginInvokeResult =
                calcOne.EndInvoke(oneAsyncResult) +
                calcTwo.EndInvoke(twoAsyncResult) +
                calcThree.EndInvoke(threeAsyncResult);

            var beginInvokeTime = DateTime.Now - start;

            // Assert.
            Assert.AreEqual(syncResult, beginInvokeResult);
            Assert.IsTrue(beginInvokeTime < syncTime);
        }

        [TestMethod]
        public void ParallelTest()
        {
            const int iterations = 20;
            const int sleep = 50;

            // Parallel.
            int parallelResult = 0;

            Action<int> func = i =>
            {
                Thread.Sleep(sleep);
                parallelResult += i;
            };

            var start = DateTime.Now;
            Parallel.For(0, iterations, func);
            var parallelTime = DateTime.Now - start;

            // Synchonous.
            var syncResult = 0;
            start = DateTime.Now;

            for (var i = 0; i < iterations; i++)
            {
                Thread.Sleep(sleep);
                syncResult += i;
            }

            var syncTime = DateTime.Now - start;

            // Assert.
            Assert.IsTrue(parallelTime < syncTime);
            Assert.AreEqual(parallelResult, syncResult);
        }

        public static double CalculationTwo(double previousResult)
        {
            return CalculationTwo() + previousResult;
        }

        public static double CalculationThree(double previousResult)
        {
            return CalculationThree() + previousResult;
        }

        [TestMethod]
        public void WithChainedTasks()
        {
            // Chained.
            var start = DateTime.Now;

            var chainedResult = Task.Factory
                .StartNew(() => CalculationOne())
                .ContinueWith(previousTask =>
                    CalculationTwo(previousTask.Result))
                .ContinueWith(previousTask =>
                    CalculationThree(previousTask.Result));

            var chainedTime = DateTime.Now - start;

            // Synchonous.
            start = DateTime.Now;

            var syncResult =
                CalculationThree(CalculationTwo(CalculationOne()));

            var syncTime = DateTime.Now - start;

            // Assert.
            Assert.IsTrue(chainedTime < syncTime);
            Assert.AreEqual(chainedResult.Result, syncResult);
            Assert.IsTrue(chainedResult.IsCompleted);
        }

        [TestMethod]
        public void WithTasks()
        {
            // Synchronous.
            var start = DateTime.Now;

            var syncResult =
                CalculationOne() +
                CalculationTwo() +
                CalculationThree();

            var syncTime = DateTime.Now - start;

            // Tasks.
            start = DateTime.Now;

            var calcOne = Task.Factory.StartNew(CalculationOne);
            var calcTwo = Task.Factory.StartNew(CalculationTwo);
            var calcThree = Task.Factory.StartNew(CalculationThree);

            Task.WaitAll(calcOne, calcTwo, calcThree);

            var tasksResult =
                calcOne.Result + calcTwo.Result + calcThree.Result;

            var tasksTime = DateTime.Now - start;

            // Assert.
            Assert.IsTrue(tasksTime < syncTime);
            Assert.AreEqual(tasksResult, syncResult);
        }

        public static double CalculationOneWithProgress
            (BackgroundWorker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException("worker");
            }

            for (var i = 1; i <= 10; ++i)
            {
                Thread.Sleep(50);
                worker.ReportProgress(i * 10);
            }

            return 1;
        }

        [SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope")]
        [TestMethod]
        public void WithBackgroundWorker()
        {
            using (var worker = new BackgroundWorker
            { WorkerReportsProgress = true })
            {
                worker.DoWork += (sender, e) =>
                {
                    var senderWorker = (BackgroundWorker)sender;
                    e.Result = CalculationOneWithProgress(senderWorker);
                };

                int progress = 0;

                worker.ProgressChanged += (sender, e) =>
                {
                    progress = e.ProgressPercentage;
                };

                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (double)e.Result;
                    Assert.AreEqual(1, result);
                    Assert.AreEqual(100, progress);
                };

                worker.RunWorkerAsync();
            }
        }

        [TestMethod]
        public void WithBeginInvokeCallbacks()
        {
            AsyncDelegate calcOne = CalculationOne;

            calcOne.BeginInvoke(asyncResult =>
            {
                var result = (AsyncResult)asyncResult;
                var caller = (AsyncDelegate)result.AsyncDelegate;
                Assert.AreEqual(1, caller.EndInvoke(result));
            }, null);
        }

        public static double CalculationOneWithProgress
            (IProgress<int> progressReporter)
        {
            if (progressReporter == null)
            {
                throw new ArgumentNullException("progressReporter");
            }

            for (var i = 1; i <= 10; ++i)
            {
                Thread.Sleep(50);
                progressReporter.Report(i * 10);
            }

            return 1;
        }

        [TestMethod]
        public void WithTasksAndProgressReport()
        {
            var progress = 0;

            var progressReporter = new Progress<int>(percent =>
            {
                progress = percent;
            });

            var result = Task.Factory
                .StartNew(() =>
                    CalculationOneWithProgress(progressReporter))
                .Result;

            Assert.AreEqual(100, progress);
            Assert.AreEqual(1, result);
        }
    }
}
