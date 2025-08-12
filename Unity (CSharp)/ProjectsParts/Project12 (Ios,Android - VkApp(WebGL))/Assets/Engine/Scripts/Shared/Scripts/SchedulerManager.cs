using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Shared
{
	/// <summary>
	/// Schedule Manager allows you to make delayed calls using coroutines.
	/// </summary>
	public class SchedulerManager
	{

		private Dictionary<int, Stack<GameObject>> m_GameObjectPool = new();
		private Dictionary<int, int> m_InstantiatedGameObjects = new();

		public static SchedulerManager Instance { get; private set; }

		public SchedulerManager()
		{
			Instance = this;
		}

		public static void Schedule(Action action, float delay)
		{

			Instance.ScheduleAsync(action, delay).Forget();
		}

		public async UniTask ScheduleAsync(Action action, float delay)
		{

			await UniTask.Delay((int)(delay * 1000));
			action?.Invoke();
		}
	}
}