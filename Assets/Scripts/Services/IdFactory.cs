using System;
using System.Collections.Generic;
using UnityEngine;

namespace EconomicsGame.Services {
	public sealed class IdFactory {
		readonly Dictionary<Type, int> _ids = new Dictionary<Type, int>();

		public int GenerateNewId<T>() {
			var type = typeof(T);
			_ids.TryGetValue(type, out var accum);
			accum++;
			_ids[type] = accum;
			Debug.Log($"GenerateNewId({type.Name}): {accum}");
			return accum;
		}

		public void AdvanceTo<T>(int value) {
			var type = typeof(T);
			_ids.TryGetValue(type, out var accum);
			_ids[type] = Math.Max(accum, value);
		}
	}
}