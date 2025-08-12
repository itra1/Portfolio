using UnityEngine;
using System.Collections;
using DG.Tweening;


namespace it.Game.Environment.Level5.RoomSymbols
{
  
  public class OnePlatform : MonoBehaviourBase
  {
	 [SerializeField]
	 private Vector3 _openPosition;
	 [SerializeField]
	 private Vector3 _closePosition;

	 [ContextMenu("SaveStartPosition")]
	 public void SaveStartPosition()
	 {
		_openPosition = transform.position;
	 }

	 [ContextMenu("SaveClosePosition")]
	 public void SaveClosePosition()
	 {
		_closePosition = transform.position;
	 }

	 public void OnReset()
	 {
		SetClose();
	 }


	 public void SetOpen() {
		transform.DOMove(_openPosition, 0.3f);
	 }

	 public void SetClose()
	 {
		transform.DOMove(_closePosition, 0.3f);
	 }

  }
}