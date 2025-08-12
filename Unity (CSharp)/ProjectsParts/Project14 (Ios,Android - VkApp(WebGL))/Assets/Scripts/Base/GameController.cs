using Core.Engine.App.Base;
using Core.Engine.Components.SaveGame;
using Core.Engine.Components.User;
using Core.Engine.Signals;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Popups.Elements;
using Core.Engine.uGUI.Screens;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameItems.Platforms;
using Scripts.Common;
using Scripts.Players;
using Scripts.Signals;
using UnityEngine;
using Zenject;

namespace Game.Base
{
	public class GameController :AppController, IGameController
	{

		private readonly IPopupProvider _popupProvider;
		private readonly IUserProvider _userProvider;
		private readonly SceneComponents _sceneComponents;
		private readonly Transform _platformTransform;
		private readonly Player _player;
		private readonly PlatformSpawner _platformGenerator;
		private readonly SaveGameProvider _saveGameProvider;
		private readonly PlatformSettings _platformSettings;
		private readonly GameLevelSG _gameLevel;

		private bool _isGame;

		public GameController(ISaveGameProvider sessionProvider
		, SignalBus bus
		, IScreensProvider screenProvider
		, SceneComponents sceneComponents
		, IPopupProvider popupProvider
		, PlatformSpawner platformgenerator
		, PlatformSettings platformSettings
		, SaveGameProvider saveGameProvider
		, IUserProvider userProvider
		) : base(sessionProvider
		, bus
		, screenProvider)
		{
			_popupProvider = popupProvider;
			_sceneComponents = sceneComponents;
			_platformTransform = _sceneComponents.PlatformParent;
			_player = _sceneComponents.Player;
			_platformGenerator = platformgenerator;
			_userProvider = userProvider;
			_saveGameProvider = saveGameProvider;
			_platformSettings = platformSettings;
			_gameLevel = (GameLevelSG)_saveGameProvider.GetProperty<GameLevelSG>();

			_signalBus.Subscribe<PlayGameSignal>(OnPlayGameSignal);
			_signalBus.Subscribe<ScreenDragSignal>(OnScreenDragSignal);
			_signalBus.Subscribe<GameExitPlaySignal>(OnGameExitPlayrSignal);

			_player.gameObject.SetActive(false);
		}

		private void OnScreenDragSignal(ScreenDragSignal signal)
		{
			if (!_isGame)
				return;
			_platformTransform.rotation *= Quaternion.Euler(0, -signal.DragDelta.x * 0.4f, 0);
		}
		private void OnPlayGameSignal(PlayGameSignal signal)
		{
			PlayGameAsync().Forget();
		}

		private async UniTask PlayGameAsync()
		{
			var levelStartPopup = _popupProvider.OpenPopup(PopupTypes.LevelStart);
			await UniTask.Delay(300);
			await ClearScene();

			var camParent = _sceneComponents.CameraParent;
			camParent.enabled = false;
			camParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			camParent.TargetPosition = Vector3.zero;

			var pp = _sceneComponents.PlatformParent;
			pp.rotation = Quaternion.identity;

			var ld = new LevelData() { Level = _gameLevel.Value, PlatformCount = _platformSettings.PlatformLevelSpawn.Random() };

			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.PlayGame });
			_signalBus.Fire(new LevelStartSignal() { Level = ld });

			_player.gameObject.SetActive(true);
			_player.IsMove = false;
			_isGame = false;
			_player.ResetPosition();
			_player.ResetCountGame();
			_player.SetLevelData(ld);
			_player.OnPlayerDamage = () => PlayerDamage().Forget();
			_player.OnComplete = () => PlayerFinish().Forget();

			_platformGenerator.ResetGame();
			_platformGenerator.SetGameData(ld);
			_platformGenerator.IsGame = true;
			_player.MeshGO.transform.localScale = Vector3.zero;

			await UniTask.Delay(1000);
			await levelStartPopup.Hide();
			await UniTask.Delay(500);
			await _player.MeshGO.transform.DOScale(Vector3.one, 0.5f).ToUniTask();

			await UniTask.Delay(500);
			camParent.enabled = true;
			_player.IsMove = true;
			_isGame = true;
		}

		private async UniTask RestartGame()
		{
			var camParent = _sceneComponents.CameraParent;
			camParent.enabled = false;
			_platformGenerator.IsGame = false;
			_player.ResetPosition();
			_player.ResetCountGame();
			camParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			camParent.TargetPosition = Vector3.zero;
			var pp = _sceneComponents.PlatformParent;
			pp.rotation = Quaternion.identity;
			_platformGenerator.ResetGame();
			_platformGenerator.IsGame = true;
			await UniTask.Delay(400);
			_player.IsMove = true;
			_isGame = true;
			camParent.enabled = true;
		}

		private async UniTask NextGame()
		{
			_gameLevel.Value++;
			//_ = RestartGame();
			_ = PlayGameAsync();
		}

		public async UniTask ExitGame()
		{
			_player.gameObject.SetActive(false);
			_player.IsMove = false;
			_player.ResetPosition();

			_platformGenerator.ResetGame();
			_platformGenerator.IsGame = false;

			_ = _screenProvider.OpenWindow(ScreenTypes.FirstPage);
			_isGame = false;
		}

		private async UniTask PlayerDamage()
		{
			_isGame = false;

			var popup = (GameOwerPopup)_popupProvider.OpenPopup(PopupTypes.GameOwer, false);

			await popup.Show();
			popup.OnRepeatAction = null;
			popup.OnRepeatAction = () =>
			{
				RestartGame().Forget();
			};

			//await UniTask.Delay(1500);
			//RestartGame().Forget();
			//await UniTask.Delay(1000);
			//popup.gameObject.SetActive(false);
		}

		private async UniTask PlayerFinish()
		{
			_isGame = false;
			var popup = (GameLevelCompletePopup)_popupProvider.OpenPopup(PopupTypes.LevelComplete, false);

			await popup.Show();
			await UniTask.Delay(1500);
			NextGame().Forget();
			await UniTask.Delay(1000);
			popup.gameObject.SetActive(false);
			//popup.OnNextAction = () =>
			//{
			//	NextGame().Forget();
			//};
		}

		private void OnGameExitPlayrSignal(GameExitPlaySignal signal)
		{
			_isGame = false;
			ExitGame().Forget();
		}

		private async UniTask ClearScene()
		{

		}

	}
}
