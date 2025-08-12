using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Handlers
{
  public class LevelPortal : MonoBehaviour
  {

    public void Use()
    {
      it.Game.Managers.GameManager.Instance.NextLevel();
    }
  }
}