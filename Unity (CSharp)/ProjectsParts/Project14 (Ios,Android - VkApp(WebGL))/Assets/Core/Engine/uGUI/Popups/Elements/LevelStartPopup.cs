using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.SaveGame;
using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Popups
{
	[PrefabName(PopupTypes.LevelStart)]
	public class LevelStartPopup : Popup
	{
		[SerializeField] private TMPro.TMP_Text _levelLabel;

		private GameLevelSG _level;

		protected override float TimeHide => 0.5f;
		private int _startSoundIndex = -1;

		[Inject]
		public void Initialize(ISaveGameProvider saveGameProvider)
		{
			_level = (GameLevelSG) saveGameProvider.GetProperty<GameLevelSG>();
		}

		protected override void BeforeShow()
		{
			base.BeforeShow();

			_startSoundIndex = ++_startSoundIndex % _soundLibrary.StartSounds.Length;

			_levelLabel.text = (_level.Value + 1).ToString();
			_ = _audioFactory.Create()
			.AutoDespawn()
			.Play(_soundLibrary.StartSounds[_startSoundIndex]);
		}
	}
}
