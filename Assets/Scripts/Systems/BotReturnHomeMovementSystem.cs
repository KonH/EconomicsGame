using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class BotReturnHomeMovementSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemService = _runtimeData.ItemService;
			var locationService = _runtimeData.LocationService;
			foreach ( var characterIdx in _filter ) {
				var shouldMoveToHome = false;
				var totalFoodCount = 0.0;
				ref var inventory = ref _filter.Get3(characterIdx);
				foreach ( var itemId in inventory.Items ) {
					var itemEntity = itemService.GetEntity(itemId);
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					totalFoodCount += item.Count.Value;
					if ( totalFoodCount <= 5 ) {
						continue;
					}
					shouldMoveToHome = true;
					break;
				}
				if ( !shouldMoveToHome ) {
					continue;
				}
				ref var character = ref _filter.Get1(characterIdx);
				var currentLocationEntity = locationService.GetEntity(character.CurrentLocation);
				if ( !currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				var possibleHomeLocations = new List<int>();
				foreach ( var locationEntity in locationService.Locations ) {
					ref var location = ref locationEntity.Get<Location>();
					if ( location.Id == character.CurrentLocation ) {
						continue;
					}
					if ( !locationEntity.Has<FoodSource>() ) {
						possibleHomeLocations.Add(location.Id);
					}
				}
				if ( possibleHomeLocations.Count == 0 ) {
					continue;
				}
				var targetLocationId = possibleHomeLocations[Random.Range(0, possibleHomeLocations.Count)];
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var moveEvent = ref characterEntity.Get<MoveCharacterActionEvent>();
				moveEvent.TargetLocation = targetLocationId;
			}
		}
	}
}