using Game.Game.Elements.Weapons.Settings;
using Game.Game.Settings;
using Game.Providers.Audio.Settings;
using Game.Providers.Avatars.Common;
using Game.Providers.Battles.Settings;
using Game.Providers.DailyBonus.Settings;
using Game.Providers.Nicknames.Settings;
using Game.Providers.Profile.Settings;
using Game.Providers.Smiles.Settings;
using Game.Providers.Telegram.Settings;
using Game.Providers.TimeBonuses.Settings;
using UnityEngine;
using Zenject;

namespace Game.Common.Settings
{
	[CreateAssetMenu(fileName = "AppSettings", menuName = "App/Create/Settings/AppSettings", order = 2)]
	public class AppSettings : ScriptableObjectInstaller<AppSettings>, IAppSettings
	{
		[SerializeField] private string _tournament;
		[SerializeField] private int _stage;

		public GameSettings GameSettings;
		public BoardsSettings TexturesSettings;
		public BattleSettings BattleSettings;
		public TimeBonusSettings TimeBonusSettings;
		public DailyBonusSettings DailyBonusSettings;
		public ProfileSettings ProfileSettings;
		public AvatarsSettings AvatarSettings;
		public AudioProviderSettings AudioSettings;
		public NicknameSettings Nicknames;
		public ResourcesIconsSettings ResourcesIcone;
		public TelegramSettings Telegram;
		public WeaponSettings WeaponSettings;
		public SmileSettings SmileSettings;

		public override void InstallBindings()
		{
#if !UNITY_EDITOR && PLATFORM_STANDALONE_WIN
			ReadTournamentsSettingsToFile();
#endif
			Container.BindInstance(GameSettings).IfNotBound();
			Container.BindInstance(TexturesSettings).IfNotBound();
			Container.BindInstance(BattleSettings).IfNotBound();
			Container.BindInstance(TimeBonusSettings).IfNotBound();
			Container.BindInstance(DailyBonusSettings).IfNotBound();
			Container.BindInstance(AvatarSettings).IfNotBound();
			Container.BindInstance(AudioSettings).IfNotBound();
			Container.BindInstance(Nicknames).IfNotBound();
			Container.BindInstance(ProfileSettings).IfNotBound();
			Container.BindInstance(ResourcesIcone).IfNotBound();
			Container.BindInstance(Telegram).IfNotBound();
			Container.BindInterfacesTo<WeaponSettings>()
				.FromInstance(WeaponSettings).AsSingle().NonLazy();
			Container.BindInterfacesTo<SmileSettings>()
				.FromInstance(SmileSettings).AsSingle().NonLazy();
		}

		//[ContextMenu("SaveTournamentsSettingsToFile")]
		//public void SaveTournamentsSettingsToFile()
		//{
		//	FileHelper.WriteAllText($"{Application.streamingAssetsPath}/TournamentsSettings.txt", Newtonsoft.Json.JsonConvert.SerializeObject(TournamentSettings));
		//}

		//[ContextMenu("ReadTournamentsSettingsToFile")]
		//public void ReadTournamentsSettingsToFile()
		//{
		//	var text = FileHelper.ReadAllText($"{Application.streamingAssetsPath}/TournamentsSettings.txt");
		//	TournamentSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleSettings>(text);
		//}

		//[System.Serializable]
		//public class PlayerLevelReward
		//{
		//	public CalculablePlayerResources Reward;
		//}

		//[ContextMenu("RecordFormation")]
		//public void RecordFormation()
		//{

		//	if (string.IsNullOrEmpty(_tournament))
		//	{
		//		Debug.LogError("Пустое поле с uuid турнира");
		//		return;
		//	}

		//	if (!TournamentSettings.Items.Exists(x => x.Uuid == _tournament))
		//	{
		//		Debug.LogError("Не корректное поле с uuid турнира");
		//		return;
		//	}
		//	var tournament = TournamentSettings.Items.Find(x => x.Uuid == _tournament);
		//	var indexTournament = TournamentSettings.Items.FindIndex(x => x.Uuid == _tournament);

		//	if (_stage < 0 || tournament.Stages.Count < _stage)
		//	{
		//		Debug.LogError("Не корректный идентификатор стадии");
		//		return;
		//	}

		//	var form = FindObjectOfType<BoardRecordFormation>();
		//	if (form == null)
		//	{
		//		Debug.LogError("Не найден сомпонент BoardRecordFormation");
		//		return;
		//	}
		//	var st = tournament.Stages[_stage];
		//	st.Formation = form.GetFormation();
		//	tournament.Stages.RemoveAt(_stage);
		//	tournament.Stages.Insert(_stage, st);
		//	TournamentSettings.Items.RemoveAt(indexTournament);
		//	TournamentSettings.Items.Insert(indexTournament, tournament);
		//}

	}
}
