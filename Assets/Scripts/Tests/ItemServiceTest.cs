using EconomicsGame.Components;
using EconomicsGame.Services;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UnityEngine;

namespace EconomicsGame.Tests {
	public sealed class ItemServiceTest {
		[Test]
		public void IsItemAssignedToEntityProvider() {
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(new EcsWorld(), entityProvider);

			service.AddInitializedItem(1, EcsEntity.Null);

			entityProvider.TryGetEntity<Item>(1, out _).Should().BeTrue();
		}

		[Test]
		public void IsInventoryItemHasValidId() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			var characterEntity = Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();
			ref var inventory = ref characterEntity.Get<Inventory>();

			var entity = service.CreateNewItemInInventory(ref character, ref inventory);

			ref var item = ref entity.Get<Item>();
			item.Id.Should().Be(1);
		}

		[Test]
		public void IsItemAddedToInventory() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			var characterEntity = Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();
			ref var inventory = ref characterEntity.Get<Inventory>();

			var itemEntity = service.CreateNewItemInInventory(ref character, ref inventory);

			ref var item = ref itemEntity.Get<Item>();
			inventory.Items.Should().Contain(item.Id);
		}

		[Test]
		public void IsInventoryItemHasValidOwner() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			var characterEntity = Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();
			ref var inventory = ref characterEntity.Get<Inventory>();

			var entity = service.CreateNewItemInInventory(ref character, ref inventory);

			ref var item = ref entity.Get<Item>();
			item.Owner.Should().Be(character.Id);
		}

		[Test]
		public void IsItemRemovedFromInventory() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			var characterEntity = Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();
			ref var inventory = ref characterEntity.Get<Inventory>();
			var itemEntity = service.CreateNewItemInInventory(ref character, ref inventory);
			ref var item = ref itemEntity.Get<Item>();

			service.RemoveFromInventory(item.Id, ref inventory);

			inventory.Items.Should().BeEmpty();
		}

		[Test]
		public void IsTradeAddedToLocation() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			ref var character = ref Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location).Get<Character>();

			service.CreateTradeAtLocation(ref character, ref location, world.NewEntity(), 1, 1);

			location.Trades.Should().Contain(1);
		}

		[Test]
		public void IsTradeRemovedFromLocation() {
			var world = new EcsWorld();
			var entityProvider = new EntityProvider();
			var service = Services.CreateItemService(world, entityProvider);
			ref var location = ref Services.CreateLocationService(world, entityProvider)
				.CreateNewLocation(Vector2.zero).Get<Location>();
			ref var character = ref Services.CreateCharacterService(world, entityProvider)
				.CreateNewCharacterInLocation(ref location).Get<Character>();
			var tradeEntity = service.CreateTradeAtLocation(ref character, ref location, world.NewEntity(), 1, 1);
			ref var item = ref tradeEntity.Get<Item>();

			service.RemoveTradeFromLocation(item.Id, ref location);

			location.Trades.Should().BeEmpty();
		}
	}
}