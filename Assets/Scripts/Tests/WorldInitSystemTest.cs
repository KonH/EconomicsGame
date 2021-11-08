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
			var world = InitWorld(null);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().NotBeEmpty();
		}

		[Test]
		public void IsWorldJustLoaded() {
			var data = CreateSampleData();

			var world = InitWorld(data);

			EcsEntity[] entities = null;
			world.GetAllEntities(ref entities);
			entities.Should().NotBeEmpty();
		}

		EcsWorld InitWorld(List<List<object>> data) {
			var runtimeData = new RuntimeData(new InMemoryStore());
			if ( data != null ) {
				runtimeData.PersistantService.Save(data);
			}
			var world = new EcsWorld();
			var systems = new EcsSystems(world);
			systems
				.Inject(runtimeData)
				.Add(new WorldInitSystem())
				.Init();
			return world;
		}

		List<List<object>> CreateSampleData() {
			var data = new List<List<object>> {
				new List<object> {
					new Location()
				},
				new List<object> {
					new Character()
				},
				new List<object> {
					new Item()
				}
			};
			return data;
		}
	}
}