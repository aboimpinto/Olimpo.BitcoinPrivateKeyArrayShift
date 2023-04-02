using System.Reactive.Subjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbQueueService;

namespace PrivateKeyShifter.Service
{
    public class PrivateKeyShifterWorker : BackgroundService
    {
        private readonly ILogger<PrivateKeyShifterWorker> _logger;
        private ISubscriber _subscriber;
        private IPublisher _publisher;

        private Subject<PrivateKeyAddress> _onNewPrivateKeyArray;

        public PrivateKeyShifterWorker(ILogger<PrivateKeyShifterWorker> logger)
        {
            this._logger = logger;

            this._onNewPrivateKeyArray = new Subject<PrivateKeyAddress>();

            var debugMode = false;

#if DEBUG
            debugMode = true;
#endif

            this._logger.LogInformation($"DebuggerMode: {debugMode}");

            this._subscriber = new Subscriber(debugMode);
            this._publisher = new Publisher(debugMode);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogInformation("PrivateKeyShifter started...");

            this._subscriber
                .SubscribeQueueCollection<PrivateKeyAddress>(stoppingToken)
                .Subscribe(x => 
                {
                    this.Shift(x.Payload.PrivateKeyBytes);
                    x.ProcessSucessful = true;
                });

            this._onNewPrivateKeyArray
                .Subscribe(async x => 
                {
                    await this._publisher.SendAsync<PrivateKeyAddress>(x);
                });

            return Task.CompletedTask;
        }

        private void Shift(byte[] sourceArray)
        {
            for (var i = 0; i < sourceArray.Length; i++)
            {
                var filler = new  byte[sourceArray.Length];
                for(var j = 0; j < sourceArray.Length; j++)
                {
                    filler[j] = sourceArray[(j + sourceArray.Length - i) % sourceArray.Length];
                }

                this._onNewPrivateKeyArray.OnNext(new PrivateKeyAddress { PrivateKeyBytes = filler });
                this._logger.LogTrace($"[{string.Join(",", filler)}]");
            }
        }
    }
}