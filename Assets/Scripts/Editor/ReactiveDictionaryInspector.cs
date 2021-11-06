using System;
using JetBrains.Annotations;
using Leopotam.Ecs;
using Leopotam.Ecs.UnityIntegration.Editor;
using UniRx;
using UnityEditor;

namespace EconomicsGame.Editor {
	[UsedImplicitly]
	sealed class ReactiveDictionaryInspector : IEcsComponentInspector {
		public Type GetFieldType() => typeof(ReactiveDictionary<string, ReactiveProperty<float>>);

		public void OnGUI(string label, object value, EcsWorld world, ref EcsEntity entityId) {
			var data = (ReactiveDictionary<string, ReactiveProperty<float>>)value;
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			using ( new EditorGUI.IndentLevelScope() ) {
				foreach ( var pair in data ) {
					EditorGUILayout.LabelField($"{pair.Key}: {pair.Value.Value:0.00}");
				}
			}
		}
	}
}