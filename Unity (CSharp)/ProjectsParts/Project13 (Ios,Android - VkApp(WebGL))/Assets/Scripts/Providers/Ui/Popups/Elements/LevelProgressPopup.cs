using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.Profile;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements
{
	[PrefabName(PopupsNames.LevelProgress)]
	public class LevelProgressPopup : Popup
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private RectTransform _progressLineBack;
		[SerializeField] private RectTransform _progressLine;

		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;
		private PlayerLevelItemView.Factory _itemsFactory;
		private List<PlayerLevelItemView> _itemsList = new();
		private float _maxHeightProgress;
		private float _itemHeight = 280;

		[Inject]
		public void Constructor(SignalBus signalBus, IProfileProvider profileProvider, PlayerLevelItemView.Factory itemsFactory)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
			_itemsFactory = itemsFactory;
		}

		protected override void Awake()
		{
			base.Awake();
			MakeList();
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			_signalBus.Subscribe<ExperienceChangeSignal>(ConfirmProgress);
			ConfirmProgress();
			ScrollActualPositing();
		}
		public void OnDisable()
		{
			_signalBus.Unsubscribe<ExperienceChangeSignal>(ConfirmProgress);
		}

		protected override void UpdateShow()
		{
			base.UpdateShow();
			ScrollActualPositing();
		}

		private void ScrollActualPositing()
		{
			if (_dialog == null)
				return;
			if (_profileProvider.CurrentLevel == 0)
			{
				_scrollRect.content.anchoredPosition = new Vector2(_scrollRect.content.anchoredPosition.x, _scrollRect.content.rect.height);
				return;
			}
			var itemRect = _itemsList.Find(x => x.Index == _profileProvider.CurrentLevel).GetComponent<RectTransform>();
			var posY = -itemRect.anchoredPosition.y - _scrollRect.GetComponent<RectTransform>().rect.height / 2;
			//Debug.Log($"target anchor {posY}");
			//Debug.Log($"target anchor {_dialog.localScale}");
			_scrollRect.content.anchoredPosition = new Vector2(_scrollRect.content.anchoredPosition.x, posY * _dialog.localScale.y);
		}

		private void MakeList()
		{
			if (_itemsList.Count > 0)
				return;
			float startPosition = -236;
			float count = _profileProvider.Levels.Count;
			for (var i = _profileProvider.Levels.Count - 1; i > 0; i--)
			{
				var inst = _itemsFactory.Create(_profileProvider.Levels[i]);
				var rt = inst.GetComponent<RectTransform>();
				rt.SetParent(_scrollRect.content);
				rt.localScale = Vector3.one;
				rt.anchoredPosition = new(_progressLineBack.anchoredPosition.x, startPosition - (_itemHeight * (_itemsList.Count)));
				_itemsList.Add(inst);
			}
			_scrollRect.content.sizeDelta = new(_scrollRect.content.sizeDelta.x, -(startPosition + startPosition - (_itemHeight * (count - 1))));
			_maxHeightProgress = _progressLineBack.rect.height - 12;
		}

		private void ConfirmProgress()
		{
			var height = (_itemHeight * _profileProvider.CurrentLevel) + (_itemHeight / _profileProvider.ExperienceInLevel * _profileProvider.CurrentExperienceInLevel);
			_progressLine.sizeDelta = new(_progressLine.sizeDelta.x, height);
		}

		public void CloseButtonTouch()
		{
			Hide().Forget();
		}
	}
}
