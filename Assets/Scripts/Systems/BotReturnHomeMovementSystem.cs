using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotReturnHomeMovementSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				var shouldMoveToHome = false;
				var totalFoodCount = 0L;
				ref var inventory = ref _filter.Get3(characterIdx);
				foreach ( var itemId in inventory.Items ) {
					Assert.IsTrue(itemProvider.TryGetEntity(itemId, out var itemEntity));
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
				Assert.IsTrue(locationProvider.TryGetEntity(character.CurrentLocation, out var currentLocationEntity));
				if ( !currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				var possibleHomeLocations = new List<int>();
				foreach ( var locationEntity in _runtimeData.Locations ) {
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