using System;
using Newtonsoft.Json;
using UniRx;

namespace EconomicsGame.Services {
	sealed class ReactivePropertyConverter : JsonConverter {
		public override bool CanConvert(Type objectType) =>
			objectType.IsGenericType &&
			objectType.GetGenericTypeDefinition() ==
			typeof(ReactiveProperty<>);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var valueProperty = value.GetType().GetProperty("Value");
			var targetValue = valueProperty?.GetValue(value);
			serializer.Serialize(writer, targetValue);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var targetType = objectType.GetGenericArguments()[0];
			var targetValue = serializer.Deserialize(reader, targetType);
			var instance = Activator.CreateInstance(objectType);
			objectType.GetProperty("Value")?.SetValue(instance, targetValue);
			return instance;
		}
	}
}