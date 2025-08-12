using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaceUI : MonoBehaviour
{
	public int Place = -1;

	[SerializeField] private RectTransform _reserveImage;

	public bool IsInverce { get; set; } = false;
	private Action<int> touchCallback = null;
	private bool _isReserve;
	public void Init(int place, Action<int> touchCallback)
	{
		Place = place;
		this.touchCallback = touchCallback;
		SetReserved(false);
	}

	public void TouchSitdown()
	{
		if (_isReserve) return;
		touchCallback?.Invoke(Place);
	}

	public void SetReserved(bool isReserve)
	{
		_isReserve = isReserve;
		_reserveImage.gameObject.SetActive(IsInverce ? !isReserve : isReserve);
	}
}
