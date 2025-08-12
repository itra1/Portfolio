using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Leaderboard.Base;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class LeaderboardPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnBackEvent;

		[SerializeField] private Button _backButton;
		[SerializeField] private ScrollRect _contentRect;
		[SerializeField] private LeaderboardItemElement _leaderboardPrefab;

		private List<LeaderboardItemElement> _instances = new();
		private DiContainer _container;
		private IProfileProvider _profileProvider;

		[Inject]
		private void Build(DiContainer container, IProfileProvider profileProvider)
		{
			_container = container;
			_profileProvider = profileProvider;
		}

		public override async UniTask<bool> Initialize()
		{
			_ = await base.Initialize();
			_backButton.onClick.AddListener(BackButtonTouch);

			return true;
		}

		public void SetData(List<LeaderboardItem> leaderboard)
		{
			_contentRect.normalizedPosition = Vector3.zero;

			while (_instances.Count < leaderboard.Count + 1)
			{
				var instance = Instantiate(_leaderboardPrefab, _contentRect.content);
				_container.Inject(instance);
				_instances.Add(instance);
			}

			foreach (var item in _instances)
			{
				item.gameObject.SetActive(false);
			}

			LeaderboardItem myProp = new()
			{
				Nickname = "You",
				Value = _profileProvider.Profile.Points,
				AvatarUuid = _profileProvider.Profile.AvatarUuid
			};

			int index = 0;
			bool isMy = false;
			for (int i = 0; i < leaderboard.Count; i++)
			{
				if (_profileProvider.Profile.Points > leaderboard[i].Value && !isMy)
				{
					index++;
					isMy = true;
					AddProp(_instances[i], index, myProp, true);
				}
				index++;
				AddProp(_instances[i], index, leaderboard[i]);
			}
			if (!isMy)
			{
				index++;
				AddProp(_instances[leaderboard.Count - 1], index, myProp, true);
			}
		}

		private void AddProp(LeaderboardItemElement inst, int index, LeaderboardItem leaderboardItem, bool isPlayer = false)
		{
			inst.SetData(index, leaderboardItem, isPlayer);
			inst.gameObject.SetActive(true);
			inst.transform.SetAsLastSibling();
		}

		private void BackButtonTouch() => OnBackEvent?.Invoke();

		protected override void PositionContent()
		{
			_positionTransform.anchoredPosition = Vector2.zero;
			_positionTransform.sizeDelta = Vector2.zero;

			var scrollrect = _positionTransform.GetComponent<ScrollRect>();
			scrollrect.content.sizeDelta = new(scrollrect.content.sizeDelta.x, (scrollrect.transform as RectTransform).rect.height);
		}
	}
}
