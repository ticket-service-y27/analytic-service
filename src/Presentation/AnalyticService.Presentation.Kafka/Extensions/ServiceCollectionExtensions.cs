using AnalyticService.Presentation.Kafka.ConsumerHandlers;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Loyalty.Kafka.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Kafka.Contracts;

namespace AnalyticService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        const string producerKey = "Presentation:Kafka:Producers";

        collection.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(consumer => consumer
                .WithKey<PaymentStatusKey>()
                .WithValue<PaymentStatusValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:PaymentStatus"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<PaymentStatusConsumerHandler>())
            .AddProducer(producer => producer
                .WithKey<PaymentsSumKey>()
                .WithValue<PaymentsSumValue>()
                .WithConfiguration(configuration.GetSection($"{producerKey}:Loyalty"))
                .SerializeKeyWithProto()
                .SerializeValueWithProto()));

        return collection;
    }

    public static IEventsConfigurationBuilder AddPresentationKafkaEventHandlers(
        this IEventsConfigurationBuilder builder)
    {
        return builder.AddHandlersFromAssemblyContaining<IKafkaAssemblyMarker>();
    }
}