using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Orleans.Runtime
{
    public class ExtensionsLoggingTelemetryConsumer: ITraceTelemetryConsumer
    {
        public ExtensionsLoggingTelemetryConsumer(ILogger logger)
        {
            Logger = logger;
        }

        internal ILogger Logger { get; }        

        public void TrackTrace(string message)
        {
            TrackTrace(message, Severity.Info, null);
        }

        public void TrackTrace(string message, Severity severity)
        {
            TrackTrace(message, severity, null);
        }

        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
            TrackTrace(message, Severity.Info, null);
        }

        public void TrackTrace(string message, Severity severity, IDictionary<string, string> properties)
        {
            LogEntry entry;
            switch (severity)
            {
                case Severity.Off:
                    break;
                case Severity.Error:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogError(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                case Severity.Warning:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogWarning(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                case Severity.Info:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogInformation(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                case Severity.Verbose:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogDebug(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                case Severity.Verbose2:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogTrace(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                case Severity.Verbose3:
                    entry = LogEntryFactory.Default.CreateLogEntry(message, properties);
                    Logger.LogTrace(entry.MessageFormat, entry.GetPropertyValues());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }        

        public void Flush()
        {
        }

        public void Close()
        {
        }        
    }
}