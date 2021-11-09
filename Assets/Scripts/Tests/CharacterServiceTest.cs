using EconomicsGame.Components;
using EconomicsGame.Services;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UnityEngine;

namespace EconomicsGame.Tests {
	public sealed class CharacterServiceTest {
		[Test]
		public void IsCharacterAssignedToEntityProvider() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);

			service.AddInitializedCharacter(1, EcsEntity.Null);

			entityProvider.TryGetEntity<Character>(1, out _).Should().BeTrue();
		}

		[Test]
		public void IsLocationCharacterHasValidId() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();

			var entity = service.CreateNewCharacterInLocation(ref location);

			ref var character = ref entity.Get<Character>();
			character.Id.Should().Be(1);
		}

		[Test]
		public void IsCharacterAddedToGlobalCollection() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();

			var entity = service.CreateNewCharacterInLocation(ref location);

			service.Characters.Should().Contain(entity);
		}

		[Test]
		public void IsCharacterAddedToLocation() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();

			var characterEntity = service.CreateNewCharacterInLocation(ref location);

			ref var character = ref characterEntity.Get<Character>();
			location.Characters.Should().Contain(character.Id);
		}

		[Test]
		public void IsLocationCharacterHasValidCurrentLocation() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();

			var entity = service.CreateNewCharacterInLocation(ref location);

			ref var character = ref entity.Get<Character>();
			character.CurrentLocation.Should().Be(location.Id);
		}

		[Test]
		public void IsCharacterRemovedFromLocation() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateCharacterService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			var characterEntity = service.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();

			service.RemoveFromLocation(character.Id, ref location);

			location.Characters.Should().BeEmpty();
		}
	}
}