using UnityEngine;

namespace EconomicsGame.UnityComponents {
	sealed class ClickRaycaster : MonoBehaviour {
		readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[4];

		Camera _camera;
		IClickTarget _target;

		void Start() {
			_camera = Camera.main;
		}

		void Update() {
			if ( !Input.GetMouseButtonDown(0) ) {
				return;
			}
			var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
			var count = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, _raycastHits);
			for ( var i = 0; i < count; i++ ) {
				var hit = _raycastHits[i];
				if ( !hit.collider ) {
					return;
				}
				var go = hit.collider.gameObject;
				if ( go.TryGetComponent(out _target) ) {
					_target.OnClick();
				}
			}
		}
	}
}