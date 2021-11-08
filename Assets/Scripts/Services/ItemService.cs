using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services {
	public sealed class ItemService {
		readonly EntityProvider _entityProvider;

		public ItemService(EntityProvider entityProvider) {
			_entityProvider = entityProvider;
		}

		public void Add(int id, EcsEntity entity) => _entityProvider.Assign<Item>(id, entity);

		public void AddToInventory(int id, EcsEntity entity, ref Inventory inventory) {
			Add(id, entity);
			inventory.Items.Add(id);
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Item>(id);

		public void RemoveFromInventory(int id, ref Inventory inventory) {
			inventory.Items.Remove(id);
			Remove(id);
		}

		public void RemoveTradeFromLocation(int id, ref Location location) {
			location.Trades.Remove(id);
			Remove(id);
		}

		void Remove(int id) => _entityProvider.Remove<Item>(id);
	}
}