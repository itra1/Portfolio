using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AfterGetWeapon2 : MonoBehaviour, IAfterGetProduct {
  public void AfterGetProduct() {
    if (SceneManager.GetActiveScene().name != "Game")
      return;

    WeaponsSpawner.Instance.SetActiveWeapon("c6345d4b-8f0c-4d59-bc5e-3cd189af2ea7");

  }
}
