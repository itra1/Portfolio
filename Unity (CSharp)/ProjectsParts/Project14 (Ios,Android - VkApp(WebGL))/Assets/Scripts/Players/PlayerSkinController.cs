using Core.Engine.Components.Skins;
using Core.Engine.Components.Themes;
using Core.Engine.Components.Themes.Common;
using Core.Engine.uGUI;
using Game.Players.Skins;
using UnityEngine;
using Zenject;

namespace Scripts.Players {
	/// <summary>
	/// Контроллер скина
	/// </summary>
	public class PlayerSkinController : MonoBehaviour, IZInjection {

		[SerializeField] private Transform _skinParent;
		[SerializeField] private Player _player;

		private GameObject _prefab;
		private SignalBus _signalBus;
		private DiContainer _diController;
		private ThemeProvider _themeProvider;
		private SkinProvider _skinProvider;

		[Inject]
		public void Constructor(SignalBus signalBus, DiContainer diController, SkinProvider skinProvider, ThemeProvider themeProvider) {
			_signalBus = signalBus;
			_diController = diController;
			_themeProvider = themeProvider;
			_skinProvider = skinProvider;

			_themeProvider.OnThemeChanged.AddListener(OnSetThemeSignal);

			if (_themeProvider.ActiveTheme != null)
				OnSetThemeSignal(_themeProvider.ActiveTheme);
			//_signalBus.Subscribe<SetThemeSignal>(OnSetThemeSignal);
		}

		public void OnSetThemeSignal(IThemeItem theme) {
			ConfirmTheme();
		}

		public void SetSkin(Color blobColor, GameObject newSkin) {

			if (_prefab)
				Destroy(_prefab);

			_player.BlobColor = blobColor;

			_prefab = Instantiate(newSkin, _skinParent);

			if (_prefab.TryGetComponent<SkinSound>(out var soundSkin)) {
				_diController.Inject(soundSkin);
				_player.SkinSound = soundSkin;
			}

			ConfirmTheme();

			if (_prefab && _prefab.TryGetComponent<Transform>(out var pTr)) {
				pTr.localPosition = Vector3.zero;
				pTr.localScale = Vector3.one;
				pTr.localRotation = Quaternion.identity;
				pTr.gameObject.SetActive(true);
			}
		}

		private void ConfirmTheme() {
			if (_prefab && _prefab.TryGetComponent<SkinMesh>(out var skin)) {
				_diController.Inject(skin);
				if (_themeProvider.ActiveTheme != null) {
					skin.Initiate(_themeProvider.ActiveTheme.Ball);
					_player.BlobColor = _themeProvider.ActiveTheme.Ball;
				}
			}

		}
	}
}
