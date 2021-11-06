using System;
using JetBrains.Annotations;
using Leopotam.Ecs;
using Leopotam.Ecs.UnityIntegration.Editor;
using UniRx;
using UnityEditor;

namespace EconomicsGame.Editor {
	[UsedImplicitly]
	sealed class ReactiveCollectionInspector : IEcsComponentInspector {
		public Type GetFieldType() => typeof(ReactiveCollection<int>);

		public void OnGUI(string label, object value, EcsWorld world, ref EcsEntity entityId) {
			var data = (ReactiveCollection<int>)value;
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			using ( new EditorGUI.IndentLevelScope() ) {
				foreach ( var element in data ) {
					EditorGUILayout.LabelField(element.ToString());
				}
			}
		}
	}
}