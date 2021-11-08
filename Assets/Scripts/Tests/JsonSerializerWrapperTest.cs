using EconomicsGame.Services;
using FluentAssertions;
using NUnit.Framework;
using UniRx;

namespace EconomicsGame.Tests {
	public sealed class JsonSerializerWrapperTest {
		struct TestPropertyStruct {
			public ReactiveProperty<int> Property;
		}

		[Test]
		public void IsNullReactivePropertyDeserializedToDefaultInstance() {
			TestPropertyStruct sourceData = new TestPropertyStruct {
				Property = null
			};
			var serializer = new JsonSerializerWrapper();
			var json = serializer.Serialize(sourceData);

			var finalData = serializer.Deserialize<TestPropertyStruct>(json);

			finalData.Property.Should().NotBeNull();
		}
	}
}