using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EconomicsGame.Services {
	sealed class Vector2JsonConverter : JsonConverter {
		public override bool CanConvert(Type objectType) => objectType == typeof(Vector2);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var vector = (Vector2)value;
			var obj = new JObject {
				{ "x", new JValue(vector.x) },
				{ "y", new JValue(vector.y) }
			};
			obj.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var vector = new Vector2();
			var isXProperty = false;
			var isYProperty = false;
			while ( reader.Read() ) {
				switch ( reader.TokenType ) {
					case JsonToken.PropertyName: {
						var propertyName = (string)reader.Value;
						isXProperty = propertyName == "x";
						isYProperty = propertyName == "y";
						break;
					}
					case JsonToken.Float: {
						var floatValue = (float)(double)reader.Value;
						if ( isXProperty ) {
							vector.x = floatValue;
						}
						if ( isYProperty ) {
							vector.y = floatValue;
						}
						break;
					}
					case JsonToken.EndObject: return vector;
				}
			}
			throw new InvalidOperationException();
		}

	}
}