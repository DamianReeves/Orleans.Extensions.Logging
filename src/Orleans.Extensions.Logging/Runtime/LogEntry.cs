using System.Linq;

namespace Orleans.Runtime
{
    internal class LogEntry
    {
        private static readonly string[] NoPropertyValues = { };
        public static readonly LogEntry Null = new LogEntry(null, null);

        public LogEntry(string messageFormat, string[] propertyValues, bool isValid = true)
        {
            MessageFormat = messageFormat;
            PropertyValues = propertyValues ?? NoPropertyValues;
            IsValid = isValid;
        }

        public bool IsValid { get; }
        public string MessageFormat { get; }

        public string[] PropertyValues { get; }

        public object[] GetPropertyValues()
        {
            return PropertyValues?.Select(x => (object) x).ToArray();
        }
    }
}