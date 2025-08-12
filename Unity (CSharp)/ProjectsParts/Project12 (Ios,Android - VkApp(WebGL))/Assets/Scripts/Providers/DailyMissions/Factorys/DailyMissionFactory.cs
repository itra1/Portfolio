using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.DailyMissions.Settings;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Factorys
{
	public class DailyMissionFactory : IDailyMissionFactory
	{
		private Dictionary<string, Type> _missionTypes = new();
		private DiContainer _diContainer;

		public DailyMissionFactory(DiContainer container)
		{
			_diContainer = container;
			FindClasses();
		}

		private void FindClasses()
		{
			var classes = (from t in Assembly.GetExecutingAssembly().GetTypes()
										 where t.IsClass
											 && !t.IsAbstract
											 && t.IsSubclassOf(typeof(Mission))
										 select t).ToList();
			_missionTypes.Clear();

			foreach (var item in classes)
			{
				var className = item.GetCustomAttribute<PrefabNameAttribute>().Name;
				_missionTypes.Add(className, item);
			}
		}

		public string RandomKey()
		{
			return _missionTypes.ElementAt(UnityEngine.Random.Range(0, _missionTypes.Count)).Key;
		}

		public IMission GetInstance(MissionItem mission, string save)
		{
			var missionInstance = (IMission) Activator.CreateInstance(_missionTypes[mission.Type]);
			_diContainer.Inject(missionInstance);
			missionInstance.SetMission(mission);
			if (!string.IsNullOrEmpty(save))
				missionInstance.SetSaveData(save);
			missionInstance.Initialize();
			return missionInstance;
		}
	}
}
