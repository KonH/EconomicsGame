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
			var hit = Physics2D.Raycast(mousePos, Vector2.zero);
			if (hit.collider) {
				var go = hit.collider.gameObject;
				if (go.TryGetComponent(out _target)) {
					_target.OnClick();
				}
			}
		}
	}
}