using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class BotFoodSourceMovementSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemService = _runtimeData.ItemService;
			var locationService = _runtimeData.LocationService;
			foreach ( var characterIdx in _filter ) {
				ref var stats = ref _filter.Get3(characterIdx);
				var shouldMoveToFoodSource = true;
				if ( stats.Values.ContainsKey("Coward") ) {
					shouldMoveToFoodSource = false;
				} else if ( !stats.Values.ContainsKey("Brave") ) {
					ref var inventory = ref _filter.Get4(characterIdx);
					foreach ( var itemId in inventory.Items ) {
						var itemEntity = itemService.GetEntity(itemId);
						ref var item = ref itemEntity.Get<Item>();
						if ( item.Name != "Food" ) {
							continue;
						}
						shouldMoveToFoodSource = false;
						break;
					}
				}
				if ( !shouldMoveToFoodSource ) {
					continue;
				}
				ref var character = ref _filter.Get1(characterIdx);
				var currentLocationEntity = locationService.GetEntity(character.CurrentLocation);
				if ( currentLocationEntity.Has<FoodSource>() ) {
					ref var currentFoodSource = ref currentLocationEntity.Get<FoodSource>();
					var isEnoughFood = (currentFoodSource.Remaining > 0) && (currentFoodSource.Locked < currentFoodSource.Remaining);
					if ( isEnoughFood ) {
						continue;
					}
				}
				var possibleFoodSourceLocations = new List<int>();
				foreach ( var locationEntity in locationService.Locations ) {
					ref var location = ref locationEntity.Get<Location>();
					if ( location.Id == character.CurrentLocation ) {
						continue;
					}
					if ( locationEntity.Has<FoodSource>() ) {
						possibleFoodSourceLocations.Add(location.Id);
					}
				}
				if ( possibleFoodSourceLocations.Count == 0 ) {
					continue;
				}
				var targetLocationId = possibleFoodSourceLocations[Random.Range(0, possibleFoodSourceLocations.Count)];
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var moveEvent = ref characterEntity.Get<MoveCharacterActionEvent>();
				moveEvent.TargetLocation = targetLocationId;
			}
		}
	}
}