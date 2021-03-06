using EconomicsGame.Components;
using EconomicsGame.Systems;
using EconomicsGame.Services;
using EconomicsGame.UnityComponents;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Startup {
	sealed class EcsStartup : MonoBehaviour, IEcsStartup {
		[SerializeField] SceneData _sceneData;
		[SerializeField] GlobalData _globalData;

		public SceneData SceneData => _sceneData;
		public GlobalData GlobalData => _globalData;

		public RuntimeData RuntimeData { get; private set; }

		EcsWorld _world;
		EcsSystems _systems;

		void Awake() {
			_world = new EcsWorld();
			RuntimeData = new RuntimeData(_world, new PersistentDataFileStore());
			foreach ( var initializer in _sceneData.Initializers ) {
				initializer.Attach(this);
			}
		}

		void Start() {
			_systems = new EcsSystems(_world);
#if UNITY_EDITOR
			Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
			Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
#endif
			_systems
				.Inject(_sceneData)
				.Inject(_globalData)
				.Inject(RuntimeData)
				.Add(new WorldInitSystem())
				.Add(new ChangeSelectedLocationByClickSystem())
				.Add(new ChangeSelectedCharacterByClickSystem())
				.Add(new SelectedLocationSystem())
				.Add(new SelectedCharacterSystem())
				.Add(new BotUseFoodSystem())
				.Add(new BotReturnHomeMovementSystem())
				.Add(new BotFoodSourceMovementSystem())
				.Add(new BotBuyFoodSystem())
				.Add(new BotSellFoodSystem())
				.Add(new BotMineFoodSystem())
				.Add(new CharacterActionUpdateSystem())
				.Add(new MoveCharacterActionStartSystem())
				.Add(new MoveCharacterActionUpdateSystem())
				.Add(new HungerSystem())
				.Add(new DeathSystem())
				.Add(new ResetSelectedCharacterByDeathSystem())
				.Add(new RemoveCharacterByDeathSystem())
				.Add(new UseFoodSystem())
				.Add(new MineFoodCharacterStartActionSystem())
				.Add(new MineFoodCharacterActionUpdateSystem())
				.Add(new SellItemSystem())
				.Add(new BuyItemSystem())
				.Add(new CleanupItemSystem())
				.Add(new SaveSystem())
				.OneFrame<EmptyItemFlag>()
				.OneFrame<LocationClickEvent>()
				.OneFrame<CharacterClickEvent>()
				.OneFrame<UseItemEvent>()
				.OneFrame<SellItemEvent>()
				.OneFrame<BuyItemEvent>()
				.OneFrame<CharacterDeathEvent>()
				.OneFrame<MoveCharacterActionEvent>()
				.OneFrame<MineFoodCharacterActionEvent>()
				.OneFrame<SaveStateEvent>()
				.Init();
			foreach ( var initializer in _sceneData.Initializers ) {
				var postInitializer = initializer as IPostInitializer;
				postInitializer?.PostInit();
			}
		}

		void Update() {
			_systems?.Run();
		}

		void OnDestroy() {
			if ( _systems == null ) {
				return;
			}
			_systems.Destroy();
			_systems = null;
			_world.Destroy();
			_world = null;
		}
	}
}