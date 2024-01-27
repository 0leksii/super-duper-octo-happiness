using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AsnParserTests;

public class LoggerSpy<T> : ILogger<T>
{
    public IList<string?> State { get; } = new List<string?>();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (state != null)
        {
            State.Add(state.ToString());
        }

        if (exception != null)
        {
            State.Add(exception.ToString());
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}