using Game.Base;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using Game.Providers.Ui.Factorys;
using Game.Providers.Ui.Items;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class ItemsAnimationPanel : MonoBehaviour
	{
		[SerializeField] private RectTransform _coinsRectPosition;
		[SerializeField] private RectTransform _dollarRectPosition;
		[SerializeField] private RectTransform _experienceRectPosition;
		[SerializeField] private RectTransform _playerStarTransform;
		[SerializeField] private RectTransform _botStarTransform;

		private UiPlayerResourcesFactory _uiPlayerResourcesFactory;
		private AudioHandler _audioHandler;
		private Vector2 _coinsAnchor;
		private Vector2 _dollarAnchor;
		private Vector2 _experienceAnchor;
		private Vector2 _playerStarAnchor;
		private Vector2 _botStarAnchor;

		[Inject]
		public void Constructor(UiPlayerResourcesFactory uiPlayerResourcesFactor, AudioHandler audioHandler)
		{
			_uiPlayerResourcesFactory = uiPlayerResourcesFactor;
			_audioHandler = audioHandler;
		}

		public void Awake()
		{
			_coinsAnchor = _coinsRectPosition.position;
			_dollarAnchor = _dollarRectPosition.position;
			_experienceAnchor = _experienceRectPosition.position;
			_playerStarAnchor = _playerStarTransform.position;
			_botStarAnchor = _botStarTransform.position;
		}

		private Vector2 TargetPosition(string key)
		{
			return key switch
			{
				RewardTypes.Coins => _coinsAnchor,
				RewardTypes.Dollar => _dollarAnchor,
				RewardTypes.Experience => _experienceAnchor,
				FlyingItemName.RoundPlayer => _playerStarAnchor,
				FlyingItemName.RoundBot => _botStarAnchor,
				_ => _coinsAnchor
			};
		}

		public void SpawnItems(string type, int increment, RectTransform point, int count = 10)
		{
			_ = _audioHandler.PlayRandomClip(SoundNames.GetResources);
			var localPoint = point.position;
			for (var i = 0; i < count; i++)
			{
				var inst = _uiPlayerResourcesFactory.GetInstance(type, transform);
				inst.SetTargetPosition(TargetPosition(type));
				var instRect = inst.transform as RectTransform;
				instRect.localScale = Vector3.one;
				instRect.localRotation = Quaternion.identity;
				instRect.position = localPoint;
				instRect.localPosition = new Vector3(instRect.localPosition.x, instRect.localPosition.y, 0);
				inst.gameObject.SetActive(true);
			}
		}
	}
}
