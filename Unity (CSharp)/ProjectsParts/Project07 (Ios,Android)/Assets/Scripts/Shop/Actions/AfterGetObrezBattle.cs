using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AfterGetObrezBattle: MonoBehaviour, IAfterGetProduct
{
  public void AfterGetProduct()
  {
    if(SceneManager.GetActiveScene().name != "Game")
      return;

    WeaponsSpawner.Instance.SetActiveWeapon("d281772c-2892-4b6f-b068-fd95b7f98d08");

  }
}
