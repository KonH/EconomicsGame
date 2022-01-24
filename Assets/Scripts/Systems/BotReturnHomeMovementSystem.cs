using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using LocationService = EconomicsGame.Services.LocationService;

namespace EconomicsGame.Systems {
	public sealed class BotReturnHomeMovementSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		ItemService _itemService;
		LocationService _locationService;

		public void Init() {
			_itemService = _runtimeData.ItemService;
			_locationService = _runtimeData.LocationService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				var shouldMoveToHome = false;
				var totalFoodCount = 0.0;
				ref var inventory = ref _filter.Get3(characterIdx);
				foreach ( var itemId in inventory.Items ) {
					var itemEntity = _itemService.GetEntity(itemId);
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
				var currentLocationEntity = _locationService.GetEntity(character.CurrentLocation);
				if ( !currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				var possibleHomeLocations = new List<int>();
				foreach ( var locationEntity in _locationService.Locations ) {
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