using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Systems;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UniRx;

namespace EconomicsGame.Tests {
	public sealed class BotUseFoodSystemTest {
		[Test]
		public void IsFoodUsedByBotCharacter() {
			var runtimeData = new RuntimeData(new InMemoryStore());
			var world = new EcsWorld();
			var systems = new EcsSystems(world)
				.Inject(runtimeData)
				.Add(new BotUseFoodSystem())
				.Add(new UseFoodSystem())
				.Add(new CleanupItemSystem());
			systems.Init();
			var characterEntity = world.NewEntity();
			ref var character = ref characterEntity.Get<Character>();
			character.Id = 1;
			runtimeData.CharacterService.Add(character.Id, characterEntity);
			characterEntity.Get<BotCharacterFlag>();
			ref var stats = ref characterEntity.Get<CharacterStats>();
			stats.Values = new ReactiveDictionary<string, ReactiveProperty<float>>();
			stats.Values.Add("Hunger", new ReactiveProperty<float>(1));
			ref var inventory = ref characterEntity.Get<Inventory>();
			inventory.Items = new ReactiveCollection<int>();
			var itemEntity = world.NewEntity();
			ref var item = ref itemEntity.Get<Item>();
			item.Id = 1;
			item.Owner = character.Id;
			item.Name = "Food";
			item.Count = new ReactiveProperty<double>(1);
			itemEntity.Get<FoodItem>();
			runtimeData.ItemService.AddToInventory(item.Id, itemEntity, ref inventory);

			systems.Run();

			item.Count.Value.Should().Be(0);
			inventory.Items.Should().BeEmpty();
		}
	}
}