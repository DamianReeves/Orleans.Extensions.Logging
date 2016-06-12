using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Orleans.Runtime;
using Xunit;

namespace Orlean.Runtime
{
    public class LogEntryFactoryTests
    {
        private static readonly string[] NoPropertyValues = {};
        internal LogEntryFactory SUT { get; } = LogEntryFactory.Default;

        [Fact]
        public void When_messageFormat_is_null_and_properties_is_null()
        {            
            var entry = SUT.CreateLogEntry(null, null);
            entry.ShouldBeEquivalentTo(LogEntry.Null);
        }

        [Fact]
        public void When_messageFormat_is_null_and_properties_is_empty()
        {
            var entry = SUT.CreateLogEntry(null, new Dictionary<string, string>());
            entry.ShouldBeEquivalentTo(LogEntry.Null);
        }

        [Fact]
        public void When_messageFormat_is_null_and_we_have_properties()
        {
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var entry = SUT.CreateLogEntry(null, properties);
            entry.ShouldBeEquivalentTo(LogEntry.Null);
        }

        [Fact]
        public void When_messageFormat_is_empty_string_and_properties_is_null()
        {
            var entry = SUT.CreateLogEntry(string.Empty, null);
            entry.ShouldBeEquivalentTo(new LogEntry(string.Empty, NoPropertyValues));
        }

        [Fact]
        public void When_messageFormat_is_empty_string_and_properties_is_empty()
        {
            var entry = SUT.CreateLogEntry(string.Empty, null);
            entry.ShouldBeEquivalentTo(new LogEntry(string.Empty, NoPropertyValues));
        }

        [Fact]
        public void When_messageFormat_is_empty_string_and_we_have_properties()
        {
            var entry = SUT.CreateLogEntry(string.Empty, null);
            entry.ShouldBeEquivalentTo(new LogEntry(string.Empty, NoPropertyValues));
        }

        [Fact]
        public void When_messageFormat_is_whitespace_only_string_then_propertyValues_should_be_empty()
        {
            var messageFormat = "       \t\t\r\n";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, NoPropertyValues));
        }

        [Fact]
        public void When_messageFormat_has_no_holes_then_propertyValues_should_be_empty()
        {
            var messageFormat = "Something happened and we have no nice details.";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, NoPropertyValues));
        }

        [Fact]
        public void When_messageFormat_has_holes_without_repeats_then_propertyValues_should_be_correct()
        {
            var messageFormat = "Logging properties {Property1} and {Property2}";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var expectedPropertyValues = new[] {"Property1Value", "Property2Value"};
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, expectedPropertyValues));
        }

        [Fact]
        public void When_messageFormat_has_holes_with_repeats_then_propertyValues_should_be_correct()
        {
            var messageFormat = "Logging properties {Property1} and {Property2}. Did I mention {Property2} and {Property1}?";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var expectedPropertyValues = new[] { "Property1Value", "Property2Value", "Property2Value", "Property1Value" };
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, expectedPropertyValues));
        }

        [Fact]
        public void When_messageFormat_has_hole_for_unsupplied_property_keep_hole_template()
        {
            var messageFormat = "Logging properties {UnknownProperty}.";
            var properties = new Dictionary<string, string>
            {
            };
            var expectedPropertyValues = new[] { "{UnknownProperty}"};
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, expectedPropertyValues));
            //entry.PropertyValues.Should().Equal(expectedPropertyValues);
        }

        [Fact]
        public void When_messageFormat_has_hole_for_unsupplied_property_mixed_with_supplied_properties()
        {
            var messageFormat = "Logging properties: {UnknownProperty}, {Property1}, {Property2}.";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var expectedPropertyValues = new[] { "{UnknownProperty}", "Property1Value", "Property2Value" };
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, expectedPropertyValues));
            //entry.PropertyValues.Should().Equal(expectedPropertyValues);
        }

        [Fact]
        public void When_messageFormat_has_hole_for_unsupplied_property_mixed_with_supplied_properties_and_repeats()
        {
            var messageFormat = "Logging properties: {UnknownProperty}, {Property1}, {Property2}. And a repeat -> {Property1}.";
            var properties = new Dictionary<string, string>
            {
                {"Property1","Property1Value" },
                {"Property2","Property2Value" },
            };
            var expectedPropertyValues = new[] { "{UnknownProperty}", "Property1Value", "Property2Value", "Property1Value" };
            var entry = SUT.CreateLogEntry(messageFormat, properties);
            entry.ShouldBeEquivalentTo(new LogEntry(messageFormat, expectedPropertyValues));
            //entry.PropertyValues.Should().Equal(expectedPropertyValues);
        }
    }
}
