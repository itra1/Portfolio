using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using it.Game.Managers;
using it.Game.Player;

namespace it.UI
{
  public class VideoScene : UIDialog
  {

	 [SerializeField]
	 private Dropdown _dropDown;
	 protected override void OnEnable()
	 {
		if (GameManager.Instance == null || GameManager.Instance.LocationManager == null)
		{
		  gameObject.SetActive(false);
		  return;
		}

		base.OnEnable();
		_dropDown.options.Clear();
		for (int i = 0; i < GameManager.Instance.LocationManager.VideoScenes.Length; i++)
		{
		  var inst = GameManager.Instance.LocationManager.VideoScenes[i];
		  _dropDown.options.Add(new Dropdown.OptionData(string.IsNullOrEmpty(inst.Title) ? inst.gameObject.name : inst.Title));
		}
	 }


	 public void Play()
	 {
		int val = _dropDown.value;
		var point = GameManager.Instance.LocationManager.VideoScenes[val];
		point.Play();
		gameObject.SetActive(false);
	 }
  }
}