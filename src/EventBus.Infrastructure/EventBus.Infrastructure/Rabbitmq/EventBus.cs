﻿using System;
using EventBus.Infrastructure.Abstractions;
using EventBus.Infrastructure.Rabbitmq.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Infrastructure.Rabbitmq
{
    public class EventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Publish<T>(T message)
            where T : class, new()
        {
            var queue = _serviceProvider.GetRequiredService<IQueue<T>>();

            queue.Connect();
            queue.Publish(message);
        }

        public void SubscribeWithAsync<T, TH>()
            where T : class, new()
            where TH : IIntegrationEventHandler<T>
        {
            var queue = _serviceProvider.GetRequiredService<IQueue<T>>();
            
            var integrationEvent = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();

            queue.Connect();
            queue.SubscribeWithAsync(async (message, model, args) =>
                await integrationEvent.HandleAsync(message).ConfigureAwait(false));
        }
    }
}
