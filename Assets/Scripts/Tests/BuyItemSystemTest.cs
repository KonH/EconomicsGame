using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Systems;
using FluentAssertions;
using Leopotam.Ecs;
using NUnit.Framework;
using UniRx;

namespace EconomicsGame.Tests {
	public sealed class BuyItemSystemTest {
		[Test]
		public void IsBoughtItemChangesOwner() {
			var runtimeData = new RuntimeData(new InMemoryStore());
			var world = new EcsWorld();
			var systems = new EcsSystems(world)
				.Inject(runtimeData)
				.Add(new BuyItemSystem());
			systems.Init();
			var sellerCharacterEntity = world.NewEntity();
			ref var sellerCharacter = ref sellerCharacterEntity.Get<Character>();
			sellerCharacter.Id = runtimeData.IdFactory.GenerateNewId<Character>();
			runtimeData.CharacterService.Add(sellerCharacter.Id, sellerCharacterEntity);
			ref var sellerInventory = ref sellerCharacterEntity.Get<Inventory>();
			sellerInventory.Items = new ReactiveCollection<int>();
			var sellerMoneyItemEntity = world.NewEntity();
			ref var sellerMoneyItem = ref sellerMoneyItemEntity.Get<Item>();
			sellerMoneyItem.Name = "Cash";
			sellerMoneyItem.Count = new ReactiveProperty<long>(1);
			sellerMoneyItem.Id = runtimeData.IdFactory.GenerateNewId<Item>();
			runtimeData.ItemService.AddToInventory(sellerMoneyItem.Id, sellerMoneyItemEntity, ref sellerInventory);
			var buyerCharacterEntity = world.NewEntity();
			ref var buyerCharacter = ref buyerCharacterEntity.Get<Character>();
			buyerCharacter.Id = runtimeData.IdFactory.GenerateNewId<Character>();
			runtimeData.CharacterService.Add(buyerCharacter.Id, buyerCharacterEntity);
			ref var buyerInventory = ref buyerCharacterEntity.Get<Inventory>();
			buyerInventory.Items = new ReactiveCollection<int>();
			var buyerMoneyItemEntity = world.NewEntity();
			ref var buyerMoneyItem = ref buyerMoneyItemEntity.Get<Item>();
			buyerMoneyItem.Name = "Cash";
			buyerMoneyItem.Count = new ReactiveProperty<long>(1);
			buyerMoneyItem.Id = runtimeData.IdFactory.GenerateNewId<Item>();
			runtimeData.ItemService.AddToInventory(buyerMoneyItem.Id, buyerMoneyItemEntity, ref buyerInventory);
			var tradeItemEntity = world.NewEntity();
			ref var tradeItem = ref tradeItemEntity.Get<Item>();
			tradeItem.Owner = sellerCharacter.Id;
			tradeItem.Id = runtimeData.IdFactory.GenerateNewId<Item>();
			tradeItem.Count = new ReactiveProperty<long>(1);
			ref var trade = ref tradeItemEntity.Get<Trade>();
			trade.PricePerUnit = 1;
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