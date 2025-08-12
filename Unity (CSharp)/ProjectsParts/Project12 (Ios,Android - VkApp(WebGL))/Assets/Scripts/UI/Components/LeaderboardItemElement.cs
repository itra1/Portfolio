using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.Leaderboard.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class LeaderboardItemElement : MonoBehaviour
	{
		[SerializeField] private TMP_Text _rankLabel;
		[SerializeField] private TMP_Text _nicknameLabel;
		[SerializeField] private TMP_Text _scoreLabel;
		[SerializeField] private Image _avatarImage;
		[SerializeField] private Image _lightImage;
		[SerializeField] private Image _medalonImage;
		[SerializeField] private Image _playerBackImage;
		[SerializeField] private Color _grayColor;
		[SerializeField] private Material _standartFontMaterial;
		[SerializeField] private Material _favoriteFontMaterial;
		[SerializeField] private ColorsStruct[] _colorsProps;

		private IAvatarsProvider _avatarProvider;
		private LeaderboardItem _leaderboardElement;
		private int _index;

		[System.Serializable]
		private struct ColorsStruct
		{
			public Color Color;
			public Sprite Medalon;
			public Material RangeFontMaterial;
		}

		[Inject]
		public void Build(IAvatarsProvider avatarProvider)
		{
			_avatarProvider = avatarProvider;
		}

		public void SetData(int index, LeaderboardItem leaderboardElement, bool isPlayer = false)
		{
			_index = index;
			_leaderboardElement = leaderboardElement;
			_rankLabel.text = _index.ToString();
			_nicknameLabel.text = _leaderboardElement.Nickname;
			_scoreLabel.text = Mathf.CeilToInt(_leaderboardElement.Value).ToString("### ### ##0");
			_avatarImage.sprite = _avatarProvider.GetAvatar(_leaderboardElement.AvatarUuid);
			_playerBackImage.gameObject.SetActive(isPlayer);

			if (_index < 4)
			{
				ColorsStruct cs = _colorsProps[_index - 1];
				_lightImage.gameObject.SetActive(true);
				_lightImage.color = cs.Color;
				_medalonImage.gameObject.SetActive(true);
				_medalonImage.sprite = cs.Medalon;
				_rankLabel.fontMaterial = _favoriteFontMaterial;
				_rankLabel.color = Color.black;
			}
			else
			{
				_lightImage.gameObject.SetActive(false);
				_medalonImage.gameObject.SetActive(false);
				_rankLabel.fontMaterial = _standartFontMaterial;
				_rankLabel.color = _grayColor;
			}
		}
	}
}
