using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Game.Handlers
{
  public class PlayerUICoordinate : MonoBehaviour
  {
	 [SerializeField]
	 private Text _coordinateText;

	 private void Update()
	 {
		if(Game.Player.PlayerBehaviour.Instance != null)
		{
		  _coordinateText.text = string.Format("x:{0} y:{1} z:{2}"
			 , Game.Player.PlayerBehaviour.Instance.transform.position.x
			 , Game.Player.PlayerBehaviour.Instance.transform.position.y
			 , Game.Player.PlayerBehaviour.Instance.transform.position.z
			 );
		}
		else
		{
		  _coordinateText.text = "NO PLAYER";
		}
	 }
  }
}