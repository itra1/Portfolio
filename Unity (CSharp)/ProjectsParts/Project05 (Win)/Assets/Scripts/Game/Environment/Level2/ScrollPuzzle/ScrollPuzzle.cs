using UnityEngine;
using System.Collections;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level2
{
  public class ScrollPuzzle : Challenge
  {
    private ScrollPuzzleItem[] _items;

    /// <summary>
    /// Открытые UUID слотов
    /// </summary>
    private string[] _openList = new string[0];


    [SerializeField]
    private string[] _itemsUUID;
    public string[] ItemsUUID { get => _itemsUUID; }

    public override bool IsInteractReady => State < 2;
    protected override void Start()
    {
      _items = GetComponentsInChildren<ScrollPuzzleItem>();
      base.Start();


      for (int i = 0; i < _items.Length; i++)
      {
        _items[i].onOpen.AddListener(OpenItem);
      }

    }
	 protected override void NoLoadData()
	 {
		base.NoLoadData();
      
      _openList = new string[0];
    }

	 public void OpenItem(ScrollPuzzleItem item)
    {
      item.onOpen.RemoveListener(OpenItem);

      string itemUuid = GetInventaryItem();

      if (string.IsNullOrEmpty(itemUuid))
        return;

      it.Game.Managers.GameManager.Instance.Inventary.Remove(itemUuid);

      System.Array.Resize(ref _openList, _openList.Length + 1);
      _openList[_openList.Length - 1] = item.Uuid;

      Save();

      CheckAllScroll();

    }

    private void SetOpenSlotsAfterLoad()
    {
      for (int i = 0; i < _items.Length; i++)
      {
        _items[i].SetClose();

        for(int x = 0; x < _openList.Length; x++)
        {
          if (_openList[x].Equals(_items[i].Uuid))
          {
            _items[i].SetOpen();
          }
        }

      }
    }

	 protected override void BeforeLoad()
	 {
		base.BeforeLoad();
      _openList = new string[0];
    }

	 public string GetInventaryItem()
    {
      for (int i = 0; i < _itemsUUID.Length; i++)
      {
        if(it.Game.Managers.GameManager.Instance.Inventary.ExistsItem(_itemsUUID[i]))
          return _itemsUUID[i];
      }
      return null;
    }

    protected override JValue SaveData()
    {
      JArray saveData = new JArray();

      for(int i = 0; i < _openList.Length; i++)
      {
        saveData.Add(_openList[i]);
      }

      return saveData;
    }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
      SetOpenSlotsAfterLoad();
    }

	 protected override void LoadData(JValue data)
    {
      StopAllCoroutines();
      base.LoadData(data);
      _openList = new string[0];

      _openList = (data as JArray).AsStringArray();

    }

    private void CheckAllScroll()
    {
      if (_openList.Length != _itemsUUID.Length)
        return;

      State = 2;

      OnComplete?.Invoke();

    }

  }
}