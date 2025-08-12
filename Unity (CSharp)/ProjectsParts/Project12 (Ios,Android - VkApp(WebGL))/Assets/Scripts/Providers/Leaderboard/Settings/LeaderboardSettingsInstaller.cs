using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Leaderboard.Settings
{
	[CreateAssetMenu(fileName = "LeaderboardSettingsInstaller", menuName = "Settings/LeaderboardSettings")]
	public class LeaderboardSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private LeaderboardSettings _settings;

		public LeaderboardSettings Settings => _settings;

		public override void InstallBindings()
		{
			_ = Container.BindInstance<LeaderboardSettings>(_settings).AsSingle().NonLazy();
		}

		[ContextMenu("Read Files")]
		public void ReadsNicknames() => _settings.ReadsNicknames();
		[ContextMenu("Read Datas")]
		public void ReadDatas() => _settings.ReadDatas();
	}
}
