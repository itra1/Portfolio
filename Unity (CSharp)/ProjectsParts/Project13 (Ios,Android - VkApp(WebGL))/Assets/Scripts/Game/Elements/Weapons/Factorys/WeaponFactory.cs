using System;
using System.Collections.Generic;
using System.Linq;
using Game.Game.Elements.Weapons.Settings;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Weapons.Factorys
{
	public class WeaponFactory : IWeaponFactory
	{
		private readonly DiContainer _diContainer;
		private readonly IWeaponSettings _settings;
		private readonly Dictionary<string, List<Weapon>> _instancesDict = new();

		private List<Weapon> _prefabsList = new();

		public List<Weapon> PrefabsList => _prefabsList;

		public WeaponFactory(DiContainer diContainer, IWeaponSettings settings)
		{
			_diContainer = diContainer;
			_settings = settings;
			LoarPrefabs();
		}

		private void LoarPrefabs()
		{
			_prefabsList = Resources.LoadAll<Weapon>(_settings.PrefabsPath).ToList();
		}

		public IWeapon GetInstance(string weaponType)
		{
			Weapon instance = null;
			if (_instancesDict.ContainsKey(weaponType))
			{
				instance = _instancesDict[weaponType].Find(x => !x.gameObject.activeInHierarchy);

				if (instance != null)
					return instance;
			}
			else
			{
				_instancesDict.Add(weaponType, new());
			}

			var instancelist = _instancesDict[weaponType];

			instance = CreateInstance(weaponType);
			instancelist.Add(instance);
			return instance;
		}

		private Weapon CreateInstance(string weaponType)
		{
			var prefab = _prefabsList.Find(x => x.Type == weaponType);

			if (prefab == null)
				throw new NullReferenceException("No exists prefab");

			prefab.gameObject.SetActive(false);

			var instance = MonoBehaviour.Instantiate(prefab);
			_instancesDict[weaponType].Add(instance);
			_diContainer.Inject(instance);

			return instance;
		}
	}
}
