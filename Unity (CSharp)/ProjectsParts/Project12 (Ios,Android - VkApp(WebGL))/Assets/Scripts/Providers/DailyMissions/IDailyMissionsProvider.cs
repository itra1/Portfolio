using System;
using System.Collections.Generic;
using Game.Scripts.App;
using Game.Scripts.Providers.DailyMissions.Common;
using UnityEngine.Events;

namespace Game.Scripts.Providers.DailyMissions
{
	public interface IDailyMissionsProvider : IApplicationLoaderItem
	{
		UnityEvent<MissionEventData> OnMissionChangeEvent { get; set; }
		DateTime DateComplete { get; }
		List<IMission> ActiveMissions { get; }
		void Reward(IMission mission);
		void AddItem(IMission mission);
	}
}