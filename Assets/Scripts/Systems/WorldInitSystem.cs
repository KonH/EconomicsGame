using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Systems {
	// TODO: refactor to proper use elements
	public sealed class WorldInitSystem : IEcsInitSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		readonly Vector2[] _locations = {
			new Vector2(-1.25f, 0), new Vector2(1.25f, 0), new Vector2(1.25f, 0.5f), new Vector2(1.25f, -0.5f)
		};

		public void Init() {
			var loader = new WorldLoader(_world, _runtimeData);
			if ( !loader.TryLoad() ) {
				Generate();
			}
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