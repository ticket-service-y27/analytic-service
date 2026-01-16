using AnalyticService.Presentation.Kafka.ConsumerHandlers;
using EventService.Infrastructure.Messaging.Contracts;
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
            .AddConsumer(consumer => consumer
                .WithKey<SeatsBookedKey>()
                .WithValue<SeatsBookedValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:Seats"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<BookedSeatsConsumerHandler>())
            .AddConsumer(consumer => consumer
                .WithKey<EventCreatedKey>()
                .WithValue<EventCreatedValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:Events"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<EventCreatedConsumerHandler>())
            .AddConsumer(consumer => consumer
                .WithKey<VenueCreatedKey>()
                .WithValue<VenueCreatedValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:Venues"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<VenueCreatedConsumerHandler>())
            .AddConsumer(consumer => consumer
                .WithKey<SeatsReturnedKey>()
                .WithValue<SeatsReturnedValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:ReturnedSeats"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<ReturnedSeatsConsumerHandler>())
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