using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Services {
	public sealed class ItemService {
		readonly EntityProvider _entityProvider;

		public ItemService(EntityProvider entityProvider) {
			_entityProvider = entityProvider;
		}

		public void Add(int id, EcsEntity entity) => _entityProvider.Assign<Item>(id, entity);

		public void AddToInventory(int id, EcsEntity entity, ref Inventory inventory) {
			Debug.Log($"AddToInventory: {entity.Get<Item>().Log()}");
			Add(id, entity);
			Assert.IsFalse(inventory.Items.Contains(id));
			inventory.Items.Add(id);
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Item>(id);

		public void RemoveFromInventory(int id, ref Inventory inventory) {
			Debug.Log($"RemoveFromInventory: IT:{id}");
			inventory.Items.Remove(id);
			Assert.IsFalse(inventory.Items.Contains(id));
			Remove(id);
		}

		public void RemoveTradeFromLocation(int id, ref Location location) {
			Debug.Log($"RemoveTradeFromLocation: IT:{id}");
			location.Trades.Remove(id);
			Assert.IsFalse(location.Trades.Contains(id));
			Remove(id);
		}

		void Remove(int id) => _entityProvider.Remove<Item>(id);
	}
}