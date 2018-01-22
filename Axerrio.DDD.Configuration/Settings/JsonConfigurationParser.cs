﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration
{
    public class JsonConfigurationParser
    {
        private JsonConfigurationParser() { }

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        private JsonTextReader _reader;

        public static IDictionary<string, string> Parse(string key, Stream input)
            => new JsonConfigurationParser().ParseStream(key, input);

        public static IDictionary<string, string> Parse(string key, string input)
            => new JsonConfigurationParser().ParseString(key, input);

        public static IDictionary<string, string> Parse<T>(string key, T input)
            => new JsonConfigurationParser().ParseObject(key, input);

        private IDictionary<string, string> ParseObject<T>(string key, T input)
        {
            var jsonConfig = JObject.FromObject(input);

            if (string.IsNullOrWhiteSpace(key))
                key = nameof(T);

            return Parse(key, jsonConfig);
        }

        private IDictionary<string, string> ParseStream(string key, Stream input)
        {
            //_data.Clear();
            _reader = new JsonTextReader(new StreamReader(input));
            _reader.DateParseHandling = DateParseHandling.None;

            var jsonConfig = JObject.Load(_reader);

            //VisitJObject(jsonConfig);

            //return _data;

            return Parse(key, jsonConfig);
        }

        private IDictionary<string, string> ParseString(string key, string input)
        {
            var jsonConfig = JObject.Parse(input);

            return Parse(key, jsonConfig);
        }

        private IDictionary<string, string> Parse(string key, JObject jObject)
        {
            _data.Clear();

             if (!string.IsNullOrWhiteSpace(key))
                EnterContext(key);

            VisitJObject(jObject);

            return _data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;

                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token.Value<JValue>());
                    break;

                default:
                    throw new FormatException("Unsupported JSON Token");
                    //throw new FormatException(Resources.FormatError_UnsupportedJSONToken(
                    //    _reader.TokenType,
                    //    _reader.Path,
                    //    _reader.LineNumber,
                    //    _reader.LinePosition));
            }
        }

        private void VisitArray(JArray array)
        {
            for (int index = 0; index < array.Count; index++)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JValue data)
        {
            var key = _currentPath;

            if (_data.ContainsKey(key))
            {
                //throw new FormatException(Resources.FormatError_KeyIsDuplicated(key));
                throw new FormatException("Key is duplicated");
            }
            _data[key] = data.ToString(CultureInfo.InvariantCulture);
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}