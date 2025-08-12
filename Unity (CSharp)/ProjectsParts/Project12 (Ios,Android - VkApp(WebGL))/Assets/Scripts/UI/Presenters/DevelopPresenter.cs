using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class DevelopPresenter : WindowPresenter, IDialogAllowed
	{
		[HideInInspector] public UnityEvent OnCloseEvent = new();
		[HideInInspector] public UnityEvent OnAddOneStarEvent = new();
		[HideInInspector] public UnityEvent OnAddOneLevelEvent = new();
		[HideInInspector] public UnityEvent OnSceneVisibleToggleEvent = new();
		[HideInInspector] public UnityEvent OnGameMissToggleEvent = new();
		[HideInInspector] public UnityEvent OnTapVisibleToggleEvent = new();
		[HideInInspector] public UnityEvent OnClearProgressButtonEvent = new();
		[HideInInspector] public UnityEvent OnGameStaticToggleEvent = new();

		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _addOneStarButton;
		[SerializeField] private Button _addOneLevelButton;
		[SerializeField] private Button _sceneVisibleToggleButton;
		[SerializeField] private Button _gameMissToggleButton;
		[SerializeField] private Button _tapVisibleToggleButton;
		[SerializeField] private Button _clearProgressButton;
		[SerializeField] private Button _gameStatsticButton;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(() => OnCloseEvent?.Invoke());

			_addOneStarButton.onClick.RemoveAllListeners();
			_addOneStarButton.onClick.AddListener(() => OnAddOneStarEvent?.Invoke());
			_addOneLevelButton.onClick.RemoveAllListeners();
			_addOneLevelButton.onClick.AddListener(() => OnAddOneLevelEvent?.Invoke());
			_sceneVisibleToggleButton.onClick.RemoveAllListeners();
			_sceneVisibleToggleButton.onClick.AddListener(() => OnSceneVisibleToggleEvent?.Invoke());
			_gameMissToggleButton.onClick.RemoveAllListeners();
			_gameMissToggleButton.onClick.AddListener(() => OnGameMissToggleEvent?.Invoke());
			_tapVisibleToggleButton.onClick.RemoveAllListeners();
			_tapVisibleToggleButton.onClick.AddListener(() => OnTapVisibleToggleEvent?.Invoke());
			_clearProgressButton.onClick.RemoveAllListeners();
			_clearProgressButton.onClick.AddListener(() => OnClearProgressButtonEvent?.Invoke());
			_gameStatsticButton.onClick.RemoveAllListeners();
			_gameStatsticButton.onClick.AddListener(() => OnGameStaticToggleEvent?.Invoke());

			return true;
		}

		public void SceneVisibleMoveSet(string value)
		{
			var label = _sceneVisibleToggleButton.GetComponentInChildren<TMP_Text>();
			label.text = $"GAME VISIBLE MODE {value}";
		}

		public void GameMissMoveSet(string value)
		{
			var label = _gameMissToggleButton.GetComponentInChildren<TMP_Text>();
			label.text = $"GAME MISS MODE {value}";
		}

		public void TapVisibleSet(bool isActive)
		{
			var label = _tapVisibleToggleButton.GetComponentInChildren<TMP_Text>();
			label.text = $"TAP VISIBLE {(isActive ? "ON" : "OFF")}";
		}

		public void GameStatisticSet(bool isActive)
		{
			var label = _gameStatsticButton.GetComponentInChildren<TMP_Text>();
			label.text = $"GAME STATISTIC {(isActive ? "ON" : "OFF")}";
		}
	}
}
