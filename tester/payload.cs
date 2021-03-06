﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    public partial class Welcome
    {
        [JsonProperty("responseId")]
        //public Guid ResponseId { get; set; }
        public string ResponseId { get; set; }

        [JsonProperty("queryResult")]
        public QueryResult QueryResult { get; set; }

        [JsonProperty("originalDetectIntentRequest")]
        public OriginalDetectIntentRequest OriginalDetectIntentRequest { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }
    }

    public partial class OriginalDetectIntentRequest
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("version")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Version { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }

    public partial class Payload
    {
        [JsonProperty("isInSandbox")]
        public bool IsInSandbox { get; set; }

        [JsonProperty("surface")]
        public Surface Surface { get; set; }

        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [JsonProperty("inputs")]
        public Input[] Inputs { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("conversation")]
        public Conversation Conversation { get; set; }

        [JsonProperty("availableSurfaces")]
        public Surface[] AvailableSurfaces { get; set; }
    }

    public partial class Surface
    {
        [JsonProperty("capabilities")]
        public Capability[] Capabilities { get; set; }
    }

    public partial class Capability
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Conversation
    {
        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Input
    {
        [JsonProperty("rawInputs")]
        public RawInput[] RawInputs { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    public partial class RawInput
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("inputType")]
        public string InputType { get; set; }
    }

    public partial class User
    {
        [JsonProperty("userStorage")]
        public string UserStorage { get; set; }

        [JsonProperty("lastSeen")]
        public DateTimeOffset LastSeen { get; set; }

        [JsonProperty("idToken")]
        public string IdToken { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }

    public partial class QueryResult
    {
        [JsonProperty("queryText")]
        public string QueryText { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("allRequiredParamsPresent")]
        public bool AllRequiredParamsPresent { get; set; }

        [JsonProperty("outputContexts")]
        public OutputContext[] OutputContexts { get; set; }

        [JsonProperty("intent")]
        public Intent Intent { get; set; }

        [JsonProperty("intentDetectionConfidence")]
        public long IntentDetectionConfidence { get; set; }

        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
    }

    public partial class Intent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public partial class OutputContext
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lifespanCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? LifespanCount { get; set; }
    }

    public partial class Parameters
    {
    }

    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
