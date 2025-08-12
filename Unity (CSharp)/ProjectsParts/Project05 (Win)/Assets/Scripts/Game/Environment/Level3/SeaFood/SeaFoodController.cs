using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using UnityEngine;
using Leguar.TotalJSON;
using it.Game.Environment.Challenges;
using DG.Tweening;

namespace it.Game.Environment.Level3
{
  public class SeaFoodController : Challenge
  {
	 [SerializeField]
	 private Material _golovartic;
	 [SerializeField]
	 private Light _golovarticLight;
	 public string[] _items;

	 [SerializeField]
	 [ColorUsage(true,true)]
	 private Color _golovasticEmis;
	 [System.Serializable]
	 public struct PieceData
	 {
		public SeaFootTarget target;
		[HideInInspector]
		public bool isExists;
	 }

	 public PieceData[] _pieces;

	 public override bool IsInteractReady => true;

	 public bool CheckInterractItemReady(int index)
	 {
		if (_pieces[index].isExists)
		  return false;

		return !string.IsNullOrEmpty(GetInventaryItem());
	 }

	 private string GetInventaryItem()
	 {
		var inventary = Game.Managers.GameManager.Instance.Inventary;
		for (int i = 0; i < _items.Length; i++)
		{
		  if (inventary.ExistsItem(_items[i]))
			 return _items[i];
		}
		return null;
	 }

	 public void ShowItem(int index)
	 {
		_pieces[index].target.VisibleFood(true, false);
		AddItem(index);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  for (int i = 0; i < _pieces.Length; i++)
		  {
			 _pieces[i].target.VisibleFood(_pieces[i].isExists, true);
		  }

		  if (State == 2)
		  {
			 _golovartic.SetColor("_EmissionColor", _golovasticEmis);
			 _golovarticLight.gameObject.SetActive(true);
		  }
		}
		if (State != 2)
		{
		  _golovartic.SetColor("_EmissionColor", Color.black);
		  _golovarticLight.gameObject.SetActive(false);
		}
	 }

	 public void AddItem(int index)
	 {
		_pieces[index].isExists = true;
		string data =GetInventaryItem();
		Game.Managers.GameManager.Instance.Inventary.Remove(data);

		Save();

		bool isFull = CheckFull();

		if (isFull)
		{
		  State = 2;
		  Save();
		  PlayComplete();
		  DOTween.To(() => _golovartic.GetColor("_EmissionColor"), (x) => _golovartic.SetColor("_EmissionColor", x), _golovasticEmis, 1);
		  float inten = _golovarticLight.intensity;
		  _golovarticLight.intensity = 0;
		  _golovarticLight.gameObject.SetActive(true);
		  _golovarticLight.DOIntensity(inten, 1f);
		}

	 }
	 private bool CheckFull(bool force = false)
	 {
		bool full = true;
		foreach (var elem in _pieces)
		{
		  if (!elem.isExists)
			 full = false;
		}

		return full;
	 }

	 [ContextMenu("Complete")]
	 public void PlayComplete()
	 {
		OnComplete?.Invoke();
	 }

	 #region Save

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		JArray pieses = data as JArray;
		for (int i = 0; i < _pieces.Length; i++)
		{
		  _pieces[i].isExists = pieses.GetBool(i);
		}

	 }

	 protected override JValue SaveData()
	 {
		JArray pieses = new JArray();
		for (int i = 0; i < _pieces.Length; i++)
		{
		  pieses.Add(_pieces[i].isExists);
		}

		return pieses;
	 }
	 #endregion

  }
}