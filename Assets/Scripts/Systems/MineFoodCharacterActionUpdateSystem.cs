using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class MineFoodCharacterActionUpdateSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MineFoodCharacterAction, CharacterActionProgress, Inventory>.Exclude<DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var idFactory = _runtimeData.IdFactory;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodAction = ref _filter.Get2(characterIdx);
				ref var actionProgress = ref _filter.Get3(characterIdx);
				var isCompleted = actionProgress.Progress >= 1;
				if ( !isCompleted ) {
					continue;
				}
				ref var inventory = ref _filter.Get4(characterIdx);
				var shouldAddItem = true;
				foreach ( var itemId in inventory.Items ) {
					Assert.IsTrue(itemProvider.TryGetComponent(itemId, out var item));
					if ( item.Name != "Food" ) {
						continue;
					}
					item.Count.Value += 1;
					shouldAddItem = false;
					break;
				}
				ref var character = ref _filter.Get1(characterIdx);
				if ( shouldAddItem ) {
					var newItemId = idFactory.GenerateNewId<Item>();
					var itemEntity = _world.NewEntity();
					ref var item = ref itemEntity.Get<Item>();
					item.Id = newItemId;
					item.Owner = character.Id;
					item.Name = "Food";
					item.Count = new ReactiveProperty<long>(1);
					ref var foodItem = ref itemEntity.Get<FoodItem>();
					foodItem.Restore = 1;
					itemProvider.Assign(newItemId, itemEntity);
					inventory.Items.Add(newItemId);
				}
				Assert.IsTrue(locationProvider.TryGetEntity(mineFoodAction.TargetLocation, out var locationEntity));
				ref var source = ref locationEntity.Get<FoodSource>();
				source.Locked--;
				source.Remaining--;
				characterEntity.Del<MineFoodCharacterAction>();
				characterEntity.Del<BusyCharacterFlag>();
				Debug.Log($"Character {character.Name} completed mining food");
			}
		}
	}
}