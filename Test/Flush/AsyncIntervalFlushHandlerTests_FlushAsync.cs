using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PostHog.Flush;
using PostHog.Model;
using PostHog.Request;
using Capture = PostHog.Model.Capture;

namespace Test.Flush
{
    [TestClass]
    public class AsyncIntervalFlushHandlerTests_FlushAsync
    {
        private Mock<IRequestHandler> _mockRequestHandler;

        private Func<Task> _requestHandlerBehavior;

        private AsyncIntervalFlushHandler _handler;

        [TestInitialize]
        public void Init()
        {
            _requestHandlerBehavior = SingleTaskResponseBehavior(Task.CompletedTask);
            _mockRequestHandler = new Mock<IRequestHandler>();

            _mockRequestHandler.Setup(r => r.MakeRequest(It.IsAny<Batch>()))
                .Returns(() => _requestHandlerBehavior())
                .Verifiable();

            _handler = GetFlushHandler(100, 20, 2000);
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_FlushAsync_ShouldNotCallMakeRequest_WhenThereAreNoEvents()
        {
            await _handler.FlushAsync();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(0));
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_FlushAsync_ShouldCallMakeRequest_WhenThereAreEvents()
        {
            _handler.Process(new Capture(null, null));
            await _handler.FlushAsync();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_FlushAsync_ShouldSplitEventInBatches()
        {
            var queueSize = 100;
            _handler = GetFlushHandler(queueSize, 20, 20000);
            await Task.Delay(100);

            for (int i = 0; i < queueSize; i++)
            {
                _handler.Process(new Alias(null));
            }

            await _handler.FlushAsync();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(5));
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_FlushAsync_ShouldWaitForPreviousFlushesTrigerredByInterval()
        {
            var time = 1500;
            _handler = GetFlushHandler(100, 20, 500);
            _requestHandlerBehavior = MultipleTaskResponseBehavior(Task.Delay(time));

            DateTime start = DateTime.Now;
            _handler.Process(new Alias(null));

            await Task.Delay(500);

            await _handler.FlushAsync();

            TimeSpan duration = DateTime.Now.Subtract(start);

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
            //50 millisecons as error margin
            Assert.IsTrue(duration.CompareTo(TimeSpan.FromMilliseconds(time - 50)) >= 0);
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_FlushAsync_ShouldLimitConcurrentProcesses()
        {
            var time = 2000;
            _handler = GetFlushHandler(100, 20, 300);
            _requestHandlerBehavior = MultipleTaskResponseBehavior(Task.Delay(time), Task.CompletedTask, Task.Delay(time));

            _handler.Process(new Alias(null));
            await Task.Delay(400);

            for (int i = 0; i < 3; i++)
            {
                _handler.Process(new Alias(null));
                _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));

                await Task.Delay(300);
            }

            await _handler.FlushAsync();

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(2));
        }

        private AsyncIntervalFlushHandler GetFlushHandler(int maxQueueSize, int maxBatchSize, int flushIntervalInMillis, int threads = 1)
        {
            return new AsyncIntervalFlushHandler(_mockRequestHandler.Object, maxQueueSize, maxBatchSize, TimeSpan.FromMilliseconds(flushIntervalInMillis), threads, "");
        }

        private Func<Task> SingleTaskResponseBehavior(Task task)
        {
            return () => task;
        }

        private Func<Task> MultipleTaskResponseBehavior(params Task[] tasks)
        {
            var response = new Queue<Task>(tasks);
            return () => response.Count > 0 ? response.Dequeue() : null;
        }
    }
}