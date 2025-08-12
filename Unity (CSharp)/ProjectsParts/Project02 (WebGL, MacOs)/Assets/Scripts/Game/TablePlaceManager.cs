using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Settings;
using it.Inputs;
using it.Helpers;
using com.ootii.Geometry;

public class TablePlaceManager : MonoBehaviour
{
	[SerializeField] private RectTransform _center;
	[SerializeField] private bool _isLobby;
	[SerializeField] private TextMeshProUGUI _tableNameLabel;
	[SerializeField] private RawImage _back;
	public string _tableName;
	public TableStyle _style;
	public string _tableKey;
	public RawImage _tableImage;
	public LobbyType Table;
	public TablePlaceTypes PlaceType;
	public List<GameType> GameTypes;
	public bool FaceToFace = false;
	public bool IsDealerChoise;
	public bool AllOrNothing;

	public PlaceController Places => _places;
	public RawImage TableImage { get => _tableImage; set => _tableImage = value; }
	public RectTransform Center { get => _center; set => _center = value; }

	private PlaceController _places;
	private it.Network.Rest.Table _table;

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		ConfirmBack();
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
	}

	private void UserProfileUpdate(com.ootii.Messages.IMessage handle)
	{
		ConfirmBack();
	}

	void ConfirmBack()
	{
		try
		{
			if (_back == null)
			{
				_back = GetComponent<RawImage>();
				if (_back == null)
					_back = gameObject.AddComponent<RawImage>();
			}
			_back.texture = _isLobby
			? it.Settings.GameSettings.GameTheme.GetBackTableTextureFast(_tableKey, true)
			:	it.Settings.GameSettings.GameTheme.BackTableTheme;
			_tableImage.texture = it.Settings.GameSettings.GameTheme.GetTableTextureFast(_style.ToString(), _tableKey);

			var arf = _back.gameObject.GetOrAddComponent<AspectRatioFitter>();
			arf.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
			arf.aspectRatio = (float)_back.texture.width / (float)_back.texture.height;

		}
		catch { }
	}

	public void SetTablePositions(it.Network.Rest.Table table){
		_table = table;

		if (_places != null)
			Destroy(_places.gameObject);

		var prefab
#if UNITY_STANDALONE
		= it.Settings.StandaloneSettings.GetTablePlace(PlaceType, table.MaxPlayers);
#elif UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		= _isLobby 
		? it.Settings.AndroidSettings.GetTableLobbyPlaceStatic(PlaceType, table.MaxPlayers)
		: it.Settings.AndroidSettings.GetGameTablePlaceStatic(PlaceType, table.MaxPlayers);
#endif

if(_tableNameLabel != null){
			var block =GameSettings.GetBlock(_table);
			_tableNameLabel.text = $"{it.Settings.GameSettings.GetFullVisibleNameOnTable(table)}\n{(_table.BuyInMinEURO).CurrencyString()} / {_table.BuyInMaxEURO.CurrencyString()}";
			//_tableNameLabel.text = $"{((GameType)_table.GameRuleId).ToString()} {_table.Name}\n{_table.BuyInMin.CurrencyString()} / {_table.BuyInMax.CurrencyString()}";
}

		GameObject inst = Instantiate(prefab.gameObject, transform);

		RectTransform rt = inst.GetComponent<RectTransform>();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		inst.transform.localPosition = Vector3.zero;
		inst.transform.localScale = Vector3.one;
		rt.anchoredPosition = Vector3.zero;
		rt.sizeDelta = Vector2.zero;
		_places = inst.GetComponent<PlaceController>();
		inst.transform.SetAsLastSibling();
		inst.gameObject.SetActive(true);
	}

}
