using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Services {
	public sealed class CharacterService {
		readonly ReactiveCollection<EcsEntity> _characters = new ReactiveCollection<EcsEntity>();

		readonly EcsWorld _world;
		readonly IdFactory _idFactory;
		readonly EntityProvider _entityProvider;

		public IReadOnlyReactiveCollection<EcsEntity> Characters => _characters;

		public CharacterService(EcsWorld world, IdFactory idFactory, EntityProvider entityProvider) {
			_world = world;
			_idFactory = idFactory;
			_entityProvider = entityProvider;
		}

		public void AddInitializedCharacter(int id, EcsEntity entity) {
			_entityProvider.Assign<Character>(id, entity);
			_characters.Add(entity);
		}

		public EcsEntity CreateNewCharacterInLocation(ref Location location) {
			var entity = _world.NewEntity();
			ref var character = ref entity.Get<Character>();
			character.Id = _idFactory.GenerateNewId<Character>();
			character.Name = $"Character {character.Id.ToString()}";
			character.Position = new ReactiveProperty<Vector2>(location.Position);
			ref var inventory = ref entity.Get<Inventory>();
			inventory.Items = new ReactiveCollection<int>();
			ref var stats = ref entity.Get<CharacterStats>();
			stats.Values = new ReactiveDictionary<string, ReactiveProperty<float>>();
			stats.Values.Add("Hunger", new ReactiveProperty<float>(0));
			stats.Values.Add("Health", new ReactiveProperty<float>(1));
			AddInitializedCharacter(character.Id, entity);
			MoveCharacterToLocation(ref character, ref location);
			return entity;
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Character>(id);

		public void RemoveFromLocation(int id, ref Location location) {
			Debug.Log($"RemoveFromLocation: CH:{id}, {location.Log()}");
			location.Characters.Remove(id);
			Remove(id);
		}

		public void MoveCharacterToLocation(ref Character character, ref Location location) {
			character.CurrentLocation = location.Id;
			location.Characters.Add(character.Id);
		}

		void Remove(int id) {
			var characterEntity = GetEntity(id);
			_characters.Remove(characterEntity);
			_entityProvider.Remove<Character>(id);
		}
	}
}