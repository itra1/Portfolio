using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using it.Game.Managers;
using it.Game.Player;

namespace it.UI
{
  public class SpawnNavigation : UIDialog
  {
	 [SerializeField]
	 private Dropdown _dropDown;
	 protected override void OnEnable()
	 {
		if(GameManager.Instance == null || GameManager.Instance.LocationManager == null)
		{
		  gameObject.SetActive(false);
		  return;
		}

		base.OnEnable();
		_dropDown.options.Clear();
		for(int i = 0; i < GameManager.Instance.LocationManager.RestorePoints.Count; i++)
		{
		  var point = GameManager.Instance.LocationManager.RestorePoints[i];
		  _dropDown.options.Add(new Dropdown.OptionData(string.IsNullOrEmpty(point.Title) ? point.gameObject.name : point.Title));
		}
		


	 }


	 public void Teleport()
	 {
		int val = _dropDown.value;
		var point = GameManager.Instance.LocationManager.RestorePoints[val];
		PlayerBehaviour.Instance.PortalJump(point.SpawnPosition);
		gameObject.SetActive(false);
	 }

  }
}