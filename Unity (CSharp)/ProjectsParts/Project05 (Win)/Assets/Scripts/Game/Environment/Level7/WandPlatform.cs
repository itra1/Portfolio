using UnityEngine;
using System.Collections;
using it.Game.Items.Inventary;
using it.Game.Player;
namespace it.Game.Environment.Level7.elevatorRoom
{
  [System.Serializable]
  public class WindActivate : UnityEngine.Events.UnityEvent<UnityEngine.Events.UnityAction>
  {

  }

  public class WandPlatform : MonoBehaviour
  {
	 [SerializeField]
	 private InventaryItem _item;
	 [SerializeField]
	 private WindActivate _onActive;

	 public void SetWand()
	 {
		_item.CheckExistsAndDeactive = false;
		_item.ColorShow(() => {
		  _onActive?.Invoke(OnComplete);
		});
	 }

	 public void OnComplete()
	 {
		_item.ColorHide(() => {
		  PlayerBehaviour.Instance.InteractionSystem.ResumeAll();
		});
	 }
  }
}