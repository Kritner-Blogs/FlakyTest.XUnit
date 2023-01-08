using System.Collections.Concurrent;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Services;

/// <summary>
/// Message bus for flaky tests
/// </summary>
/// <remarks>
/// <para>
/// This message bus only writes messages to the default xunit message bus in cases
/// where the test passed, or the maximum retries have been attempted.
/// </para>
/// <para>
/// Such a decorator? interceptor? is needed, because without it, each failed attempt would be written to the test case
/// output.  Going this route, only the *last* attempt of the test case is written. 
/// </para>
/// </remarks>
public class FlakyMessageBus : IMessageBus
{
    private readonly IMessageBus _messageBus;
    private readonly ConcurrentQueue<IMessageSinkMessage> _messageQueue = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageBus">The default XUnit message bus to intercept/decorate.</param>
    public FlakyMessageBus(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Enqueues a message
    /// </summary>
    /// <param name="message">The message to enqueue</param>
    public bool QueueMessage(IMessageSinkMessage message)
    {
        _messageQueue.Enqueue(message);
        return true;
    }

    /// <summary>
    /// Write the test case messages to the default message bus.  This is only called on success test case dispositions,
    /// or in cases where the flaky test has failed the maximum number of times.
    /// </summary>
    public void Flush()
    {
        while (_messageQueue.TryDequeue(out IMessageSinkMessage message))
        {
            _messageBus.QueueMessage(message);
        }
    }

    /// <summary>
    /// Do nothing, underlying bus is managed by xunit
    /// </summary>
    public void Dispose() { }
}
