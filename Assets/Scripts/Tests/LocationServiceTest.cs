using EconomicsGame.Components;
using EconomicsGame.Services;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UnityEngine;

namespace EconomicsGame.Tests {
	public sealed class LocationServiceTest {
		[Test]
		public void IsLocationAssignedToEntityProvider() {
			var entityProvider = new EntityProvider();
			var service = Services.CreateLocationService(new EcsWorld(), entityProvider);

			service.AddInitializedLocation(1, EcsEntity.Null);

			entityProvider.TryGetEntity<Location>(1, out _).Should().BeTrue();
		}

		[Test]
		public void IsLocationHasValidId() {
			var service = Services.CreateLocationService(new EcsWorld(), new EntityProvider());

			var entity = service.CreateNewLocation(Vector2.zero);

			ref var location = ref entity.Get<Location>();
			location.Id.Should().Be(1);
		}

		[Test]
		public void IsLocationAddedToCollection() {
			var service = Services.CreateLocationService(new EcsWorld(), new EntityProvider());

			var entity = service.CreateNewLocation(Vector2.zero);

			service.Locations.Should().Contain(entity);
		}
	}
}