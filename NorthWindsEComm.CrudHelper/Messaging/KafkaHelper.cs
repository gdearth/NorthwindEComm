using System.Text.Json;
using System.Text.Json.Nodes;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.CrudHelper.Messaging;

/// <inheritdoc/>
public class KafkaHelper<T> : IKafkaHelper<T> where T : class, IIdModel
{
    private readonly string _topicName = typeof(T).Name;
    private readonly IProducer<string, string> _publisher;
    private readonly ILogger<KafkaHelper<T>> _logger;

    public KafkaHelper(IProducer<string, string> publisher, ILogger<KafkaHelper<T>> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc/>
    public virtual async Task<bool> PublishAsync(EventType eventType, T model, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Kafka Publish Event Type: {EventType}, Message: {KafkaMessage}, ", eventType, model))
        {
            string messageString = SerializeObject(model, eventType);
            Message<string, string> message = new Message<string, string>
                { Value = messageString, Key = model.Id.ToString() };
            DeliveryResult<string, string> deliveryResult = await _publisher.ProduceAsync(topic: _topicName,
                message: message, cancellationToken: cancellationToken);
            _logger.LogInformation("Delivery Result: {DeliveryResult}", deliveryResult);

            return deliveryResult.Status switch
            {
                PersistenceStatus.Persisted => true,
                PersistenceStatus.NotPersisted => false,
                PersistenceStatus.PossiblyPersisted => true,  // fuck if I know
                _ => false
            };
        }
    }

    protected virtual string SerializeObject(T @object, EventType eventType)
    {
        var jsonString = JsonSerializer.Serialize(@object);
        var jsonObject = JsonNode.Parse(jsonString)?.AsObject();
        if (jsonObject is null)
            return jsonString;
        
        var eventTypeString = eventType switch
        {
            EventType.Create => nameof(EventType.Create).ToLower(),
            EventType.Update => nameof(EventType.Update).ToLower(),
            EventType.Delete => nameof(EventType.Delete).ToLower(),
            _ => string.Empty
        };
        
        jsonObject["eventType"] = eventTypeString;
        return jsonString;
    }
}