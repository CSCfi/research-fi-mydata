// Copyright 2015-2023 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace Serilog.Sinks.Http.TextFormatters;

/// <summary>
/// JSON formatter serializing log events into a normal format with its data normalized. The
/// lack of a rendered message means improved network load compared to
/// <see cref="NormalRenderedTextFormatter"/>. Often this formatter is complemented with a log
/// server that is capable of rendering the messages of the incoming log events.
/// </summary>
/// <seealso cref="NormalRenderedTextFormatter" />
/// <seealso cref="CompactTextFormatter" />
/// <seealso cref="CompactRenderedTextFormatter" />
/// <seealso cref="NamespacedTextFormatter" />
/// <seealso cref="ITextFormatter" />
public class WoodLogMetadataFormatter : ITextFormatter
{
    /// <summary>
    /// Gets or sets a value indicating whether the message is rendered into JSON.
    /// </summary>
    protected bool IsRenderingMessage { get; set; }

    /// <summary>
    /// Format the log event into the output.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        try
        {
            var buffer = new StringWriter();
            FormatContent(logEvent, buffer);

            // If formatting was successful, write to output
            output.WriteLine(buffer.ToString());
        }
        catch (Exception e)
        {
            LogNonFormattableEvent(logEvent, e);
        }
    }

    private void FormatContent(LogEvent logEvent, TextWriter output)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));

        output.Write("{\"Timestamp\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("o"));

        output.Write("\",\"Level\":\"");
        output.Write(logEvent.Level);

        output.Write("\",\"MessageTemplate\":");
        JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

        if (IsRenderingMessage)
        {
            output.Write(",\"RenderedMessage\":");

            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, output);
        }

        if (logEvent.Exception != null)
        {
            output.Write(",\"Exception\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        if (logEvent.Properties.Count != 0)
        {
            WriteProperties(logEvent.Properties, output);
        }

        // Better not to allocate an array in the 99.9% of cases where this is false
        var tokensWithFormat = logEvent.MessageTemplate.Tokens
            .OfType<PropertyToken>()
            .Where(pt => pt.Format != null);

        // ReSharper disable once PossibleMultipleEnumeration
        if (tokensWithFormat.Any())
        {
            // ReSharper disable once PossibleMultipleEnumeration
            WriteRenderings(tokensWithFormat.GroupBy(pt => pt.PropertyName), logEvent.Properties, output);
        }

        output.Write('}');
    }

    private static void WriteProperties(
        IReadOnlyDictionary<string, LogEventPropertyValue> properties,
        TextWriter output)
    {
        output.Write(",\"Properties\":{");

        var precedingDelimiter = string.Empty;

        /*
         * Handle Wood log metadata specific properties and construct
         * metadata for Wood log server.
         * 
         * Values are grabbed from Serilog configuration.
         * 
         * Wood project number:
         *     Serilog.Properties.WoodLogProjectNumber
         * Wood use case:
         *     Serilog.Properties.WoodLogUseCase
         * Wood retention months:
         *     Serilog.Properties.WoodLogRetentionMonths
         * 
         * Properties are excluded from the "Properties" section of
         * outgoing JSON log message. Instead, a new top level
         * key "wood" is added and it's value contains Wood metadata. 
         * 
         * Example output:
         * 
         * {
         *     "Timestamp": "2023-09-01T07:49:00.0700050Z",
         *     "Level": "Information",
         *     "MessageTemplate": "Application starting up",
         *     "Properties": {
         *         "MachineName": "devel123",
         *         "Application": "CSC.MydataApi"
         *     },
         *     "wood": {
         *         "project_number": "12341234",
         *         "use_case": "mydatatest",
         *         "retention_months": "1"
         *     }
         * }
         */
        KeyValuePair<string, LogEventPropertyValue> woodProjectNumber = new();
        KeyValuePair<string, LogEventPropertyValue> woodUseCase = new();
        KeyValuePair<string, LogEventPropertyValue> woodRetentionMonths = new();

        foreach (var property in properties)
        {
            if (property.Key.ToLower() == "woodlogprojectnumber")
            {
                woodProjectNumber = property;
            }
            else if (property.Key.ToLower() == "woodlogusecase")
            {
                woodUseCase = property;
            }
            else if (property.Key.ToLower() == "woodlogretentionmonths")
            {
                woodRetentionMonths = property;
            }
            else
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                ValueFormatter.Instance.Format(property.Value, output);
            }
        }

        output.Write('}');

        // Write Wood log metadata
        output.Write(",\"wood\":{");
        // Project number
        JsonValueFormatter.WriteQuotedJsonString("project_number", output);
        output.Write(':');
        ValueFormatter.Instance.Format(woodProjectNumber.Value, output);
        output.Write(',');
        // Use case
        JsonValueFormatter.WriteQuotedJsonString("use_case", output);
        output.Write(':');
        ValueFormatter.Instance.Format(woodUseCase.Value, output);
        output.Write(',');
        // Retention months
        JsonValueFormatter.WriteQuotedJsonString("retention_months", output);
        output.Write(':');
        ValueFormatter.Instance.Format(woodRetentionMonths.Value, output);
        output.Write('}');
    }

    private static void WriteRenderings(
        IEnumerable<IGrouping<string, PropertyToken>> tokensWithFormat,
        IReadOnlyDictionary<string, LogEventPropertyValue> properties,
        TextWriter output)
    {
        output.Write(",\"Renderings\":{");

        var rdelim = string.Empty;
        foreach (var ptoken in tokensWithFormat)
        {
            output.Write(rdelim);
            rdelim = ",";

            JsonValueFormatter.WriteQuotedJsonString(ptoken.Key, output);
            output.Write(":[");

            var fdelim = string.Empty;
            foreach (var format in ptoken)
            {
                output.Write(fdelim);
                fdelim = ",";

                output.Write("{\"Format\":");
                JsonValueFormatter.WriteQuotedJsonString(format.Format, output);

                output.Write(",\"Rendering\":");
                var sw = new StringWriter();
                format.Render(properties, sw);
                JsonValueFormatter.WriteQuotedJsonString(sw.ToString(), output);
                output.Write('}');
            }

            output.Write(']');
        }

        output.Write('}');
    }

    private static void LogNonFormattableEvent(LogEvent logEvent, Exception e)
    {
        SelfLog.WriteLine(
            "Event at {0} with message template {1} could not be formatted into JSON and will be dropped: {2}",
            logEvent.Timestamp.ToString("o"),
            logEvent.MessageTemplate.Text,
            e);
    }
}