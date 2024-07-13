using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace csharp
{
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

        [Fact]
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

            var parallelStopwatch = Stopwatch.StartNew();
            Parallel.For(0, iterations, func);
            parallelStopwatch.Stop();

            // Synchronous.
            var syncResult = 0;
            var syncStopwatch = Stopwatch.StartNew();

            for (var i = 0; i < iterations; ++i)
            {
                Thread.Sleep(sleep);
                syncResult += i;
            }

            syncStopwatch.Stop();

            // Assert.
            Assert.True(parallelStopwatch.ElapsedMilliseconds < syncStopwatch.ElapsedMilliseconds);
        }

        public static double CalculationTwo(double previousResult) =>
            CalculationTwo() + previousResult;

        public static double CalculationThree(double previousResult) =>
            CalculationThree() + previousResult;

        [Fact]
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

            // Synchronous.
            start = DateTime.Now;

            var syncResult =
                CalculationThree(CalculationTwo(CalculationOne()));

            var syncTime = DateTime.Now - start;

            // Assert.
            Assert.True(chainedTime < syncTime);
            Assert.Equal(chainedResult.Result, syncResult);
            Assert.True(chainedResult.IsCompleted);
        }

        [Fact]
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
            Assert.True(tasksTime < syncTime);
            Assert.Equal(tasksResult, syncResult);
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
        [Fact]
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
                    Assert.Equal(1, result);
                    Assert.Equal(100, progress);
                };

                worker.RunWorkerAsync();
            }
        }

        public static double CalculationOneWithProgress
            (IProgress<int> progressReporter)
        {
            if (progressReporter == null)
            {
                throw new ArgumentNullException("progressReporter");
            }

            for (var i = 1; i < 11; ++i)
            {
                Thread.Sleep(50);
                progressReporter.Report(i * 10);
            }

            return 1;
        }

        /// <summary>
        /// Note that the progress may not reach 100%,
        /// as the thread may have completed by the time the progress has updated.
        /// </summary>
        [Fact]
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

            Assert.True(progress >= 90);
            Assert.Equal(1, result);
        }
    }
}
