using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

	public RectTransform lineLoader;

	// Use this for initialization
	void Start () {
		lineLoader.sizeDelta = new Vector2(0, lineLoader.sizeDelta.y);
		StartCoroutine(LoaderScene());
		
	}

  private readonly float loadBarWidth = 341;
	
	IEnumerator LoaderScene() {

    AsyncOperation oper = null;

    float process = 0;

    while(process < loadBarWidth / 2) {
      yield return null;
      process += 7f;
      lineLoader.sizeDelta = new Vector2(process, lineLoader.sizeDelta.y);
    }


    if (!PlayerPrefs.HasKey("firstLoad"))
    {

      var weapon = WeaponManager.Instance.GetWeaponByUuid("171fccbe-a21c-4f1d-bb84-a78317213b6e");

      weapon.weaponData.IsByed = true;
      weapon.weaponData.IsEquipped = true;

      Location loc = LocationManager.Instance.GetLocationByIndex(0);
      
      UserManager.Instance.ActiveLocation = loc;
      oper = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
      PlayerPrefs.SetInt("firstLoad", 1);

    } else {

      oper = SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
      
    }

    while (!oper.isDone) {
      lineLoader.sizeDelta = new Vector2(process + oper.progress * (loadBarWidth/2), lineLoader.sizeDelta.y);
      yield return null;
    }

    SceneManager.UnloadScene("Loader");

  }
  
}
