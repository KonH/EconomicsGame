using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;

namespace EconomicsGame.Services {
	public sealed class CharacterService {
		readonly ReactiveCollection<EcsEntity> _characters = new ReactiveCollection<EcsEntity>();
		readonly EntityProvider _entityProvider;

		public IReadOnlyReactiveCollection<EcsEntity> Characters => _characters;

		public CharacterService(EntityProvider entityProvider) {
			_entityProvider = entityProvider;
		}

		public void Add(int id, EcsEntity entity) {
			_entityProvider.Assign<Character>(id, entity);
			_characters.Add(entity);
		}

		public void AddToLocation(int id, EcsEntity entity, ref Location location) {
			Add(id, entity);
			location.Characters.Add(id);
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Character>(id);

		public void RemoveCharacterFromLocation(int id, ref Location location) {
			location.Characters.Remove(id);
			Remove(id);
		}

		void Remove(int id) {
			var characterEntity = GetEntity(id);
			_characters.Remove(characterEntity);
			_entityProvider.Remove<Character>(id);
		}
	}
}