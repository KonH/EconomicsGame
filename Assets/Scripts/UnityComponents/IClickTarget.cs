namespace EconomicsGame.UnityComponents {
	// Default OnMouseDown implementation does not handle overlapping colliders properly
	// So we need to workaround that
	interface IClickTarget {
		public void OnClick();
	}
}