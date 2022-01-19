using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Systems;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Tests {
	public sealed class BuyItemSystemTest {
		[Test]
		public void IsBoughtItemChangesOwner() {
			var world = new EcsWorld();
			var runtimeData = new RuntimeData(world, new InMemoryStore());
			var systems = new EcsSystems(world)
				.Inject(runtimeData)
				.Add(new BuyItemSystem());
			systems.Init();
			runtimeData.MarketService.CreateMarket();
			var locationEntity = runtimeData.LocationService.CreateNewLocation(Vector2.zero);
			ref var location = ref locationEntity.Get<Location>();
			var characterService = runtimeData.CharacterService;
			var itemService = runtimeData.ItemService;
			var sellerCharacterEntity = characterService.CreateNewCharacterInLocation(ref location);
			ref var sellerCharacter = ref sellerCharacterEntity.Get<Character>();
			ref var sellerInventory = ref sellerCharacterEntity.Get<Inventory>();
			sellerInventory.Items = new ReactiveCollection<int>();
			var sellerMoneyItemEntity = itemService.CreateNewItemInInventory(ref sellerCharacter, ref sellerInventory);
			itemService.InitCashItem(sellerMoneyItemEntity, 1);
			var buyerCharacterEntity = characterService.CreateNewCharacterInLocation(ref location);
			ref var buyerCharacter = ref buyerCharacterEntity.Get<Character>();
			ref var buyerInventory = ref buyerCharacterEntity.Get<Inventory>();
			buyerInventory.Items = new ReactiveCollection<int>();
			var buyerMoneyItemEntity = itemService.CreateNewItemInInventory(ref buyerCharacter, ref buyerInventory);
			itemService.InitCashItem(buyerMoneyItemEntity, 1);
			var sourceEntity = world.NewEntity();
			sourceEntity.Get<Item>();
			var tradeItemEntity = itemService.CreateTrade(ref sellerCharacter, sourceEntity, 1, 1);
			ref var buyEvent = ref tradeItemEntity.Get<BuyItemEvent>();
			buyEvent.Buyer = buyerCharacter.Id;
			buyEvent.Count = 1;

			systems.Run();

			buyerInventory.Items.Count.Should().Be(2);
			var boughtItemId = buyerInventory.Items[1];
			var boughtItem = runtimeData.ItemService.GetEntity(boughtItemId).Get<Item>();
			boughtItem.Owner.Should().Be(buyerCharacter.Id);
		}
	}
}