using Leopotam.Ecs;
using UniRx;

namespace EconomicsGame.Services {
	public sealed class RuntimeData {
		public PersistantService PersistantService { get; }
		public IdFactory IdFactory { get; } = new IdFactory();

		public LocationService LocationService { get; }
		public CharacterService CharacterService { get; }
		public MarketService MarketService { get; }
		public ItemService ItemService { get; }
		public CashService CashService { get; }

		public ReactiveProperty<EcsEntity> SelectedLocation { get; } = new ReactiveProperty<EcsEntity>();
		public ReactiveProperty<EcsEntity> SelectedCharacter { get; } = new ReactiveProperty<EcsEntity>();

		public RuntimeData(EcsWorld world, IStore store) {
			PersistantService = new PersistantService(store);
			var entityProvider = new EntityProvider();
			LocationService = new LocationService(world, IdFactory, entityProvider);
			CharacterService = new CharacterService(world, IdFactory, entityProvider);
			MarketService = new MarketService(world);
			ItemService = new ItemService(world, IdFactory, entityProvider, MarketService);
			CashService = new CashService(ItemService);
		}
	}
}