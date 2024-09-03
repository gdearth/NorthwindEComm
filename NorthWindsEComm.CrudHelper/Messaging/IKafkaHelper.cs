using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.CrudHelper.Messaging;

/// <summary>
/// Represents a helper interface for publishing messages to Kafka.
/// </summary>
/// <typeparam name="T">The type of the model to publish.</typeparam>
public interface IKafkaHelper<T> where T : class, IIdModel
{
    /// <summary>
    /// Publishes a message to Kafka asynchronously.
    /// </summary>
    /// <param name="eventType">The type of the event being published (Create, Update, or Delete).</param>
    /// <param name="model">The model to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result is true if the message was successfully published, false otherwise.</returns>
    Task<bool> PublishAsync(EventType eventType, T model, CancellationToken cancellationToken);
}