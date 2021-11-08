using Leopotam.Ecs;
using UniRx;

namespace EconomicsGame.Services {
	public sealed class RuntimeData {
		public PersistantService PersistantService { get; }
		public IdFactory IdFactory { get; } = new IdFactory();

		public LocationService LocationService { get; }
		public CharacterService CharacterService { get; }
		public ItemService ItemService { get; }

		public ReactiveProperty<EcsEntity> SelectedLocation { get; } = new ReactiveProperty<EcsEntity>();
		public ReactiveProperty<EcsEntity> SelectedCharacter { get; } = new ReactiveProperty<EcsEntity>();

		public RuntimeData(IStore store) {
			PersistantService = new PersistantService(store);
			var entityProvider = new EntityProvider();
			LocationService = new LocationService(entityProvider);
			CharacterService = new CharacterService(entityProvider);
			ItemService = new ItemService(entityProvider);
		}
	}
}