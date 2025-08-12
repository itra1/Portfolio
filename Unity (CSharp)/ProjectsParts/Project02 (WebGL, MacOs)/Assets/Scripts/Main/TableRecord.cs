using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using it.Inputs;
using Sett = it.Settings;
using UnityEngine.EventSystems;
using it.Animations;
//using System.Threading.Tasks;
using it.UI;
using System.Collections;
using it.Main.SinglePages;
using UnityEngine.Events;

namespace it.Main
{
  public class TableRecord : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, MyTableRecord
	{
    public UnityEngine.Events.UnityAction<TableRecord> OnClick;
    public UnityEngine.Events.UnityAction<TableRecord> OnDoubleClick;
		public UnityEngine.Events.UnityAction<Table> OnOpen;
		UnityAction<Table> MyTableRecord.OnOpenEvent { get => OnOpen; set => OnOpen = value; }
		UnityAction MyTableRecord.OnSizeChangeEvent { get; set; }

		public Table Table { get => _table; set => _table = value; }

    public Image _hoverImage;
    public Image _focusRecord;
    // public GameObject _separate;

    //public Image _fishkaImage;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _gameLabel;
    [SerializeField] private CurrencyLabel _stakesLabel;
    [SerializeField] private CurrencyLabel _buyInLabel;
    [SerializeField] private TMValueIntAnimate _playersMinLabel;
    [SerializeField] private TextMeshProUGUI _playersMaxLabel;
    [SerializeField] private ImageCircleFillAmount _playersDiagramm;
    //public PlayersCell _playersLabel;
    //public TextMeshProUGUI _waitLabel;
    [SerializeField] private CurrencyLabel _avgPotLabel;
    //public TextMeshProUGUI _allInLabel;
    [SerializeField] private TextMeshProUGUI _vPipLabel;
    //public TextMeshProUGUI _hhrLabel;

    private Table _table;
    private string _chanel;
    private float _timeDoubleClick;

		private void OnEnable()
		{
      _timeDoubleClick = Time.realtimeSinceStartup;
      _hoverImage.gameObject.SetActive(false);
      _focusRecord.gameObject.SetActive(false);
    }

		private void OnDisable()
		{
      StopAllCoroutines();
		}

		public void SetData(Table record)
		{
			if (_table != null)
				_table.OnUpdateEvent.RemoveListener(UpdateRecord);
			_table = record;
      _chanel = SocketClient.GetChanelTable(_table.id);

			_table.OnUpdateEvent.AddListener(UpdateRecord);
			ConfirmData();
    }

		//public void SetData(TablePlayerSession session){
		//  TableManager.Instance.GetTableById(session.TableId);
		//}
		private void UpdateRecord(UpdatedMaterial updateTable)
		{
			Table t = (Table)updateTable;

      if (!gameObject.activeInHierarchy) return;

			if (_table.id != t.id) return;
			ConfirmData();
		}
    private void ConfirmData()
    {
      if (!gameObject.activeSelf) return;
      StartCoroutine(ConfirmDataCoroutine());
    }
			private IEnumerator ConfirmDataCoroutine(){

      yield return null;
      var cColor = _table.is_dealer_choice 
      ? Sett.GameSettings.Colors.Find(x => x.TypeGame.Contains(GameType.DealerChoice))
      : Sett.GameSettings.Colors.Find(x => x.TypeGame.Contains((GameType)_table.game_rule_id));
      Color recColor = cColor.Color;

			string tableNameType = cColor.Name;
			if (_table.is_all_or_nothing && tableNameType != "Holdem")
				tableNameType = "Omaha";
			//_fishkaImage.color = recColor;
			if (_nameLabel != null)
      {
        _nameLabel.text = _table.name;
#if UNITY_EDITOR
        _nameLabel.text = _table.id + " : " + _nameLabel.text;
#endif
      }
      if (_gameLabel != null)
      {
        _gameLabel.text = tableNameType;
        _gameLabel.GetComponent<TextMeshProUGUI>().color = recColor;
      }
      if (_stakesLabel != null)
      {
      if(_table.ante != null && _table.ante > 0)
          _stakesLabel.SetValue("{0} (A{1})", (float)_table.ante, (float)_table.ante);
        else
          _stakesLabel.SetValue("{0} / {1}",_table.SmallBlindSize, _table.big_blind_size);

        _stakesLabel.GetComponent<TextMeshProUGUI>().color = recColor;
      }
      if (_buyInLabel != null)
        _buyInLabel.SetValue("{0}",_table.BuyInMinEURO);
      //if (_waitLabel != null)
      //  _waitLabel.text = "5";
      if (_avgPotLabel != null)
        _avgPotLabel.SetValue(_table.average_pot);
      //if (_allInLabel != null)
      //  _allInLabel.text = "-";
      if (_vPipLabel != null)
        _vPipLabel.text = (_table.average_vpip ?? 0).ToString() + "%";
      if (_playersDiagramm != null)
      {
        _playersDiagramm.StartValue = _playersDiagramm.CurrentValue;
        _playersDiagramm.EndValue = (float)_table.table_player_sessions.Length == 0 ? 0 :(float)_table.table_player_sessions.Length / (float)_table.MaxPlayers;
        //_playersDiagramm.fillAmount = (float)_table.TablePlayerSessions.Length / (float)_table.MaxPlayers;
      }

      if (_playersMinLabel != null)
      {
        _playersMinLabel.StartValue = _playersMinLabel.EndValue;
        _playersMinLabel.EndValue = _table.table_player_sessions.Length;
        //_playersMinLabel.text = _table.TablePlayerSessions.Length.ToString();
      }
      if (_playersMaxLabel != null)
        _playersMaxLabel.text = _table.MaxPlayers.ToString();

      //if (_playersLabel != null)
      //  _playersLabel.SetData(_table.MaxPlayers, _table.CurrentPlayersCount);
    }

    public void Focus(bool isSelect)
    {
      _focusRecord.gameObject.SetActive(isSelect);
      //  Color c = Color.white;
      //c.a = isSelect ? 0.1f : 0f;
      //GetComponent<Image>().color = c;
    }
    public void ClickButton(){
      OnClick?.Invoke(this);

      if (Time.realtimeSinceStartup - _timeDoubleClick <= 0.4f)
        OnDoubleClick?.Invoke(this);

      _timeDoubleClick = Time.realtimeSinceStartup;
    }

    public void OpenButtonTouch(){
      OnOpen?.Invoke(_table);
    }

		public void OnPointerEnter(PointerEventData eventData)
		{
      _hoverImage.gameObject.SetActive(true);

    }

		public void OnPointerExit(PointerEventData eventData)
    {
      _hoverImage.gameObject.SetActive(false);
    }
	}
}