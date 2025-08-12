using System.Collections.Generic;
using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Settings;
using Game.Scripts.Controllers.AccuracyLabels.Items;
using UnityEngine;

namespace Game.Scripts.Controllers.AccuracyLabels.Factorys
{
	public class AccuracyLabelsFactory : IAccuracyLabelsFactory
	{
		private readonly INoteAccuracySettings _noteAccuracySettings;
		private readonly ISceneAccuracy _sceneAccuracy;
		private Dictionary<string, AccuracyLabel> _prefabs = new();

		public AccuracyLabelsFactory(
			INoteAccuracySettings noteAccuracySettings,
			ISceneAccuracy sceneAccuracy
		)
		{
			_noteAccuracySettings = noteAccuracySettings;
			_sceneAccuracy = sceneAccuracy;

			LoadResources();
		}

		private void LoadResources()
		{
			var prefabs = Resources.LoadAll<AccuracyLabel>(_noteAccuracySettings.AccuracyLabelsResources);

			foreach (var item in prefabs)
			{
				if (!_prefabs.ContainsKey(item.Accuracy))
				{
					var instance = MonoBehaviour.Instantiate(item, _sceneAccuracy.AccuracySpawnPoint);
					instance.gameObject.SetActive(false);
					_prefabs.Add(item.Accuracy, instance);
				}
			}
		}

		public AccuracyLabel GetInstance(string accuracy)
		 => _prefabs[accuracy];
	}
}
