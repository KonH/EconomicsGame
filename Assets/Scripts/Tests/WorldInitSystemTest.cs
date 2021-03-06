using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Systems;
using Leopotam.Ecs;
using NUnit.Framework;
using FluentAssertions;

namespace EconomicsGame.Tests {
	public sealed class WorldInitSystemTest {
		[Test]
		public void IsWorldJustGenerated() {
			var world = InitWorld(new EcsWorld(), null);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().NotBeEmpty();
		}

		[Test]
		public void IsWorldJustLoaded() {
			var data = CreateSampleData();

			var world = InitWorld(new EcsWorld(), data);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().NotBeEmpty();
		}

		[Test]
		public void IsLocationLoaded() {
			var world = new EcsWorld();
			var runtimeData = new RuntimeData(world, new InMemoryStore());
			var data = CreateSampleData(location: new Location { Id = 1 });

			InitWorld(world, runtimeData, data);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().Contain(e => IsValidLocation(e));
			runtimeData.LocationService.Locations.Should().NotBeEmpty();
			runtimeData.LocationService.GetEntity(1).IsAlive().Should().BeTrue();
			runtimeData.IdFactory.GenerateNewId<Location>().Should().Be(2);
		}

		bool IsValidLocation(EcsEntity e) =>
			e.Has<Location>() && e.Get<Location>().Id == 1;

		[Test]
		public void IsCharacterLoaded() {
			var world = new EcsWorld();
			var runtimeData = new RuntimeData(world, new InMemoryStore());
			var data = CreateSampleData(character: new Character { Id = 1 });

			InitWorld(world, runtimeData, data);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().Contain(e => IsValidCharacter(e));
			runtimeData.CharacterService.Characters.Should().NotBeEmpty();
			runtimeData.CharacterService.GetEntity(1).IsAlive().Should().BeTrue();
			runtimeData.IdFactory.GenerateNewId<Character>().Should().Be(2);
		}

		bool IsValidCharacter(EcsEntity e) =>
			e.Has<Character>() && e.Get<Character>().Id == 1;

		[Test]
		public void IsItemLoaded() {
			var world = new EcsWorld();
			var runtimeData = new RuntimeData(world, new InMemoryStore());
			var data = CreateSampleData(item: new Item { Id = 1 });

			InitWorld(world, runtimeData, data);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().Contain(e => IsValidItem(e));
			runtimeData.ItemService.GetEntity(1).IsAlive().Should().BeTrue();
			runtimeData.IdFactory.GenerateNewId<Item>().Should().Be(2);
		}

		bool IsValidItem(EcsEntity e) =>
			e.Has<Item>() && e.Get<Item>().Id == 1;

		EcsWorld InitWorld(EcsWorld world, RuntimeData runtimeData, List<List<object>> data) {
			if ( data != null ) {
				runtimeData.PersistantService.Save(data);
			}
			var systems = new EcsSystems(world);
			systems
				.Inject(runtimeData)
				.Add(new WorldInitSystem())
				.Init();
			return world;
		}

		EcsWorld InitWorld(EcsWorld world, List<List<object>> data) =>
			InitWorld(world, new RuntimeData(world, new InMemoryStore()), data);

		List<List<object>> CreateSampleData(Location location = default, Character character = default, Item item = default) {
			var data = new List<List<object>> {
				new List<object> {
					location
				},
				new List<object> {
					character
				},
				new List<object> {
					item
				}
			};
			return data;
		}
	}
}