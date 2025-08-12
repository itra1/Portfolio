using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace it.Mobile.Lobby
{
	public class LobbyFilterPanel : MonoBehaviour
	{
		public UnityEngine.Events.UnityEvent<SearchLobbyType> OnSearch;

		[SerializeField] private RectTransform _bodyRect;
		[SerializeField] private Toggle _freeToggle;
		[SerializeField] private Toggle _microToggle;
		[SerializeField] private Toggle _averageToggle;
		[SerializeField] private Toggle _highToggle;

		private RectTransform _rt;
		private SearchLobbyType _searchType;
		private bool _emit;
		private bool _isVisible;

		private void Awake()
		{
			_rt = GetComponent<RectTransform>();
			_freeToggle.onValueChanged.RemoveAllListeners();
			_freeToggle.onValueChanged.AddListener((val) =>
			{
				ChangeValue(_freeToggle, SearchLobbyType.FreeTables);
			});
			_microToggle.onValueChanged.RemoveAllListeners();
			_microToggle.onValueChanged.AddListener((val) =>
			{
				ChangeValue(_microToggle, SearchLobbyType.Micro);
			});
			_averageToggle.onValueChanged.RemoveAllListeners();
			_averageToggle.onValueChanged.AddListener((val) =>
			{
				ChangeValue(_averageToggle, SearchLobbyType.Average);
			});
			_highToggle.onValueChanged.RemoveAllListeners();
			_highToggle.onValueChanged.AddListener((val) =>
			{
				ChangeValue(_highToggle, SearchLobbyType.High);
			});
		}

		private void OnEnable()
		{
			_emit = false;
			Clear();
			_emit = true;
			_isVisible = false;
			_rt.sizeDelta = new Vector2(_rt.sizeDelta.x, 0);
			_bodyRect.gameObject.SetActive(false);
		}

		public void VisibleToggle(){
			_isVisible = !_isVisible;
			if (_isVisible)
				Visible();
			else
				Hide();
		}

		private void Visible()
		{
			_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, _bodyRect.rect.height), 0.2f).OnStart(()=> {
				_bodyRect.gameObject.SetActive(true);
			});
		}

		private void Hide()
		{
			_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, 0), 0.2f).OnComplete(()=> {
				_bodyRect.gameObject.SetActive(false);
			});
		}

		public void SearchButtonTouch()
		{
			OnSearch?.Invoke(_searchType);
		}

		private void ChangeValue(Toggle toggle, SearchLobbyType type)
		{
			if (toggle.isOn)
				_searchType |= type;
			else
				_searchType ^= type;
		}

		private void Clear()
		{
			_freeToggle.isOn = false;
			_microToggle.isOn = false;
			_averageToggle.isOn = false;
			_highToggle.isOn = false;
		}

	}
}