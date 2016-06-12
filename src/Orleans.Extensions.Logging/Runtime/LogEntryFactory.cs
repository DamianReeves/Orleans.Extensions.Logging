using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orleans.Runtime
{
    internal sealed class LogEntryFactory
    {
        public static readonly LogEntryFactory Default = new LogEntryFactory();

        private LogEntryFactory()
        {            
        }

        public LogEntry CreateLogEntry(string messageFormat, IDictionary<string, string> properties)
        {
            if (messageFormat == null)
            {
                return LogEntry.Null;
            }
            properties = properties ?? new Dictionary<string, string>();
            var propertyValues = new LinkedList<string>();
            var propertyKeyBuilder = new StringBuilder();
            var textRun = new StringBuilder();
            var state = ParseState.Text;

            // Clears the state            
            Action clear = () =>
            {
                textRun.Clear();
                propertyKeyBuilder.Clear();
            };
            Action<char> beginHole = c =>
            {
                clear();
                state = ParseState.InHole;
                textRun.Append(c);
            };
            Action endhole = () =>
            {
                clear();
                state = ParseState.Text;
            };
            for (int i = 0; i < messageFormat.Length; i++)
            {
                var ch = messageFormat[i];
                switch (ch)
                {
                    case '{':
                    {
                        switch (state)
                        {
                            case ParseState.Text:
                                beginHole(ch);
                                break;
                            case ParseState.InHole:
                                // We are in uncharted and unexpected waters now (this really isn't supported, thar be dragons!, don't know what to expect)
                                textRun.Append(ch);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }break;
                    case '}':
                        switch (state)
                        {
                            case ParseState.Text:
                                break;
                            case ParseState.InHole:
                                textRun.Append(ch);
                                var propertyKey = propertyKeyBuilder.ToString();
                                string propertyValue;
                                if (properties.TryGetValue(propertyKey, out propertyValue))
                                {
                                    propertyValues.AddLast(propertyValue);
                                }
                                else
                                {
                                    // Didn't find it so just add a substitution for the text
                                    propertyValues.AddLast(textRun.ToString());
                                }
                                endhole();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        switch (state)
                        {
                            case ParseState.Text:
                                break;
                            case ParseState.InHole:
                                textRun.Append(ch);
                                propertyKeyBuilder.Append(ch);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                }
            }

            return new LogEntry(messageFormat, propertyValues.ToArray());
        }

        private enum ParseState
        {
            Text,
            InHole,
        }
    }
}