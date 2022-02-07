using System;
using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Services {
	public sealed class ItemService {
		readonly EcsWorld _world;
		readonly IdFactory _idFactory;
		readonly EntityProvider _entityProvider;
		readonly MarketService _marketService;

		public ItemService(EcsWorld world, IdFactory idFactory, EntityProvider entityProvider, MarketService marketService) {
			_world = world;
			_idFactory = idFactory;
			_entityProvider = entityProvider;
			_marketService = marketService;
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Item>(id);

		public void AddInitializedItem(int id, EcsEntity entity) => _entityProvider.Assign<Item>(id, entity);

		public EcsEntity CreateNewItemInInventoryByCopy(
			ref Character character, ref Inventory inventory, EcsEntity sourceEntity) =>
			CreateNewItemInInventory(ref character, ref inventory, sourceEntity.Copy(), null);

		public EcsEntity CreateNewItemInInventory(
			ref Character character, ref Inventory inventory, Action<EcsEntity> initializer = null) =>
			CreateNewItemInInventory(ref character, ref inventory, _world.NewEntity(), initializer);

		EcsEntity CreateNewItemInInventory(
			ref Character character, ref Inventory inventory, EcsEntity entity, Action<EcsEntity> initializer) {
			ref var item = ref entity.Get<Item>();
			PreInitItem(ref item, ref character);
			Debug.Log($"CreateNewItemInInventory: {item.Log()} for {character.Log()} inventory");
			AddInitializedItem(item.Id, entity);
			Assert.IsFalse(inventory.Items.Contains(item.Id));
			initializer?.Invoke(entity);
			inventory.Items.Add(item.Id);
			return entity;
		}

		public void RemoveFromInventory(int id, ref Inventory inventory) {
			Debug.Log($"RemoveFromInventory: IT:{id}");
			inventory.Items.Remove(id);
			Assert.IsFalse(inventory.Items.Contains(id));
			Remove(id);
		}

		public EcsEntity CreateTrade(
			ref Character character, EcsEntity sourceEntity, double count, double pricePerUnit) {
			var entity = sourceEntity.Copy();
			ref var item = ref entity.Get<Item>();
			PreInitItem(ref item, ref character);
			AddInitializedItem(item.Id, entity);
			InitCount(ref item, count);
			ref var trade = ref entity.Get<Trade>();
			trade.PricePerUnit = pricePerUnit;
			Debug.Log($"CreateTrade: {item.Log()} x{item.Count} by {trade.PricePerUnit} from {character.Log()} character");
			ref var market = ref _marketService.Market;
			Assert.IsFalse(market.Trades.Contains(item.Id));
			market.Trades.Add(item.Id);
			return entity;
		}

		public void RemoveTrade(int id) {
			Debug.Log($"RemoveTrade: IT:{id}");
			ref var market = ref _marketService.Market;
			market.Trades.Remove(id);
			Assert.IsFalse(market.Trades.Contains(id));
			Remove(id);
		}

		void PreInitItem(ref Item item, ref Character character) {
			item.Id = _idFactory.GenerateNewId<Item>();
			item.Owner = character.Id;
		}

		public void InitCashItem(EcsEntity entity, double count) {
			InitItem(entity, "Cash", count);
		}

		public void InitFoodItem(EcsEntity entity, double count, float restore) {
			InitItem(entity, "Food", count);
			entity.Get<FoodItem>().Restore = restore;
		}

		public void InitCount(EcsEntity entity, double count) =>
			InitCount(ref entity.Get<Item>(), count);

		public void TryConsume(ref EcsEntity entity, ref Item item) {
			if ( item.Count.Value == 0 ) {
				entity.Get<EmptyItemFlag>();
			}
		}

		void InitItem(EcsEntity entity, string name, double count) {
			ref var item = ref entity.Get<Item>();
			item.Name = name;
			InitCount(ref item, count);
		}

		void InitCount(ref Item item, double count) {
			item.Count = new ReactiveProperty<double>(count);
		}

		void Remove(int id) => _entityProvider.Remove<Item>(id);
	}
}