using Engine.Scripts.Settings;
using UnityEngine;

namespace Game.Game.SpecialPoints
{
	public class SpecialPointsHandler
	{
		private readonly SpecialPointsSettings _settings;
		private SpecialPoint[] _items;

		public SpecialPointsHandler(SpecialPointsSettings settings)
		{
			_settings = settings;
			LoadResources();
		}

		private void LoadResources()
		{
			_items = Resources.LoadAll<SpecialPoint>(_settings.ResourcesPath);

			Debug.Log($"Special items {_items.Length}");
		}

		public SpecialPoint GetPrefab(string name)
		{
			for (int i = 0; i < _items.Length; i++)
			{
				if (_items[i].Name == name)
				{
					return _items[i];
				}
			}
			return _items[0];
		}
	}
}
