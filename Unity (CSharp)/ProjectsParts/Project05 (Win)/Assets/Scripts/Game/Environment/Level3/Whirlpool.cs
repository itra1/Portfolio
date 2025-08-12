using UnityEngine;
using System.Collections;
namespace it.Game.Environment
{
  public class Whirlpool : MonoBehaviour
  {

    public void PlayerEnter()
    {
      it.Game.Managers.GameManager.Instance.NextLevel(9,false);
    }
  }
}