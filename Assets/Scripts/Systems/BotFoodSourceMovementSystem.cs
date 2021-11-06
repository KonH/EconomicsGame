using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotFoodSourceMovementSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var stats = ref _filter.Get3(characterIdx);
				var shouldMoveToFoodSource = true;
				if ( stats.Values.ContainsKey("Coward") ) {
					shouldMoveToFoodSource = false;
				} else if ( !stats.Values.ContainsKey("Brave") ) {
					ref var inventory = ref _filter.Get4(characterIdx);
					foreach ( var itemId in inventory.Items ) {
						Assert.IsTrue(itemProvider.TryGetEntity(itemId, out var itemEntity));
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
				Assert.IsTrue(locationProvider.TryGetEntity(character.CurrentLocation, out var currentLocationEntity));
				if ( currentLocationEntity.Has<FoodSource>() ) {
					ref var currentFoodSource = ref currentLocationEntity.Get<FoodSource>();
					var isEnoughFood = (currentFoodSource.Remaining > 0) && (currentFoodSource.Locked < currentFoodSource.Remaining);
					if ( isEnoughFood ) {
						continue;
					}
				}
				var possibleFoodSourceLocations = new List<int>();
				foreach ( var locationEntity in _runtimeData.Locations ) {
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