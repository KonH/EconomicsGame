using System.Reflection;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	// TODO: refactor to proper use elements
	public sealed class WorldInitSystem : IEcsInitSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		readonly Vector2[] _locations = {
			new Vector2(-1.25f, 0), new Vector2(1.25f, 0), new Vector2(1.25f, 0.5f), new Vector2(1.25f, -0.5f)
		};

		public void Init() {
			if ( _runtimeData.PersistantService.CanLoad ) {
				Load();
			} else {
				Generate();
			}
		}

		void Load() {
			// TODO: to custom system/service
			var data = _runtimeData.PersistantService.Load();
			var copyComponentDataGenericMethod = typeof(WorldInitSystem).GetMethod(nameof(CopyComponentData), BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.IsNotNull(copyComponentDataGenericMethod);
			foreach ( var entityData in data ) {
				var entity = _world.NewEntity();
				foreach ( var componentData in entityData ) {
					var componentType = componentData.GetType();
					var copyComponentDataMethod = copyComponentDataGenericMethod.MakeGenericMethod(componentType);
					copyComponentDataMethod.Invoke(this, new [] { entity, componentData });
					if ( componentType == typeof(Location) ) {
						ref var location = ref entity.Get<Location>();
						_runtimeData.IdFactory.AdvanceTo<Location>(location.Id);
						_runtimeData.LocationProvider.Assign(location.Id, entity);
						_runtimeData.Locations.Add(entity);
					} else if ( componentType == typeof(Character) ) {
						ref var character = ref entity.Get<Character>();
						_runtimeData.IdFactory.AdvanceTo<Character>(character.Id);
						_runtimeData.CharacterProvider.Assign(character.Id, entity);
						_runtimeData.Characters.Add(entity);
					} else if ( componentType == typeof(Item) ) {
						ref var item = ref entity.Get<Item>();
						_runtimeData.IdFactory.AdvanceTo<Item>(item.Id);
						_runtimeData.ItemProvider.Assign(item.Id, entity);
					}
				}
			}
		}

		void CopyComponentData<T>(EcsEntity entity, T data) where T : struct {
			ref var component = ref entity.Get<T>();
			component = data;
		}

		// TODO: to custom system/service
		void Generate() {
			var idFactory = _runtimeData.IdFactory;
			var locationProvider = _runtimeData.LocationProvider;
			var characterProvider = _runtimeData.CharacterProvider;
			var itemProvider = _runtimeData.ItemProvider;
			var first = true;
			foreach ( var locationSetup in _locations ) {
				var locationEntity = _world.NewEntity();
				ref var location = ref locationEntity.Get<Location>();
				location.Id = idFactory.GenerateNewId<Location>();
				location.Name = $"Location {location.Id.ToString()}";
				location.Position = locationSetup;
				location.Characters = new ReactiveCollection<int>();
				location.Trades = new ReactiveCollection<int>();

				if ( !first ) {
					ref var source = ref locationEntity.Get<FoodSource>();
					source.Remaining = 10;
				}
				locationProvider.Assign(location.Id, locationEntity);
				_runtimeData.Locations.Add(locationEntity);

				if ( !first ) {
					continue;
				}
				for ( var i = 0; i < 5; i++ ) {
					var characterEntity = _world.NewEntity();
					ref var character = ref characterEntity.Get<Character>();
					character.Id = idFactory.GenerateNewId<Character>();
					character.Name = $"Character {character.Id.ToString()}";
					character.CurrentLocation = location.Id;
					character.Position = new ReactiveProperty<Vector2>(location.Position);
					if ( first ) {
						characterEntity.Get<PlayerCharacterFlag>();
						first = false;
					} else {
						characterEntity.Get<BotCharacterFlag>();
					}
					ref var inventory = ref characterEntity.Get<Inventory>();
					inventory.Items = new ReactiveCollection<int>();

					{
						var itemEntity = _world.NewEntity();
						ref var item = ref itemEntity.Get<Item>();
						item.Id = idFactory.GenerateNewId<Item>();
						item.Owner = character.Id;
						item.Name = "Food";
						item.Count = new ReactiveProperty<long>(1);
						ref var foodItem = ref itemEntity.Get<FoodItem>();
						foodItem.Restore = 1;
						itemProvider.Assign(item.Id, itemEntity);
						inventory.Items.Add(item.Id);
					}
					{
						var itemEntity = _world.NewEntity();
						ref var item = ref itemEntity.Get<Item>();
						item.Id = idFactory.GenerateNewId<Item>();
						item.Owner = character.Id;
						item.Name = "Cash";
						item.Count = new ReactiveProperty<long>(100);
						itemProvider.Assign(item.Id, itemEntity);
						inventory.Items.Add(item.Id);
					}

					ref var stats = ref characterEntity.Get<CharacterStats>();
					stats.Values = new ReactiveDictionary<string, ReactiveProperty<float>>();
					stats.Values.Add("Hunger", new ReactiveProperty<float>(0));
					stats.Values.Add("Health", new ReactiveProperty<float>(1));
					if ( i == 1 ) {
						stats.Values.Add("Brave", new ReactiveProperty<float>(0));
					}
					if ( i == 2 ) {
						stats.Values.Add("Coward", new ReactiveProperty<float>(0));
					}

					characterProvider.Assign(character.Id, characterEntity);
					location.Characters.Add(character.Id);
					_runtimeData.Characters.Add(characterEntity);
				}
			}
		}
	}
}