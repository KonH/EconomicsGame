using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Systems;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Tests {
	public sealed class BotUseFoodSystemTest {
		[Test]
		public void IsFoodUsedByBotCharacter() {
			var world = new EcsWorld();
			var runtimeData = new RuntimeData(world, new InMemoryStore());
			var systems = new EcsSystems(world)
				.Inject(runtimeData)
				.Add(new BotUseFoodSystem())
				.Add(new UseFoodSystem())
				.Add(new CleanupItemSystem());
			systems.Init();

			var locationEntity = runtimeData.LocationService.CreateNewLocation(Vector2.zero);
			ref var location = ref locationEntity.Get<Location>();
			var characterEntity = runtimeData.CharacterService.CreateNewCharacterInLocation(ref location);
			ref var character = ref characterEntity.Get<Character>();
			characterEntity.Get<BotCharacterFlag>();
			ref var stats = ref characterEntity.Get<CharacterStats>();
			stats.Values = new ReactiveDictionary<string, ReactiveProperty<float>>();
			stats.Values["Hunger"] = new ReactiveProperty<float>(1);
			ref var inventory = ref characterEntity.Get<Inventory>();
			inventory.Items = new ReactiveCollection<int>();
			var itemEntity = runtimeData.ItemService.CreateNewItemInInventory(ref character, ref inventory);
			runtimeData.ItemService.InitFoodItem(itemEntity, 1, 1);

			systems.Run();

			itemEntity.Get<Item>().Count.Value.Should().Be(0);
			inventory.Items.Should().BeEmpty();
		}
	}
}