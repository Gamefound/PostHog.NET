using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PostHog.Flush;
using PostHog.Model;
using PostHog.Request;

namespace Test.Flush
{
    [TestClass]
    public class AsyncIntervalFlushHandlerTests_Process
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
        public async Task AsyncIntervalFlushHandlerTests_Process_IntervalFlushShouldBeTriggeredPeriodically()
        {
            var interval = 600;
            _handler = GetFlushHandler(100, 20, interval);
            await Task.Delay(100);
            int trials = 5;

            for (int i = 0; i < trials; i++)
            {
                _handler.Process(new Alias(null));
                _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(i));
                await Task.Delay(interval);
            }

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(trials));
        }

        [TestMethod]
        public async Task AsyncIntervalFlushHandlerTests_Process_ShouldMakeRequestsWhenQueueIsFull()
        {
            var queueSize = 10;
            _handler = GetFlushHandler(queueSize, 20, 20000);
            await Task.Delay(50);

            for (int i = 0; i < queueSize + 1; i++)
            {
                _handler.Process(new Alias(null));
            }

            _mockRequestHandler.Verify(r => r.MakeRequest(It.IsAny<Batch>()), times: Times.Exactly(1));
        }

        private AsyncIntervalFlushHandler GetFlushHandler(int maxQueueSize, int maxBatchSize, int flushIntervalInMillis, int threads = 1)
        {
            return new AsyncIntervalFlushHandler(_mockRequestHandler.Object, maxQueueSize, maxBatchSize, TimeSpan.FromMilliseconds(flushIntervalInMillis), threads, "");
        }

        private Func<Task> SingleTaskResponseBehavior(Task task)
        {
            return () => task;
        }
    }
}