using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public sealed class LocationWorldView : MonoBehaviour, IClickTarget {
		EcsEntity _entity;

		public void Init(EcsEntity entity) {
			_entity = entity;
		}

		public void OnClick() {
			_entity.Get<LocationClickEvent>();
		}
	}
}