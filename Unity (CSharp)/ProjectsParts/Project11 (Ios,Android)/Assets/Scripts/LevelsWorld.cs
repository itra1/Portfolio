using System.Collections;
using System.Collections.Generic;
using GameCompany;
using UnityEngine;

public class LevelsWorld: WorldAbstract {

  public GameObject elementPrefab;
  private List<LevelsWievElement> instLevelsList = new List<LevelsWievElement>();
  private List<LevelsWievElement> levelsList = new List<LevelsWievElement>();

  private float _diffX = 1.6f;
  private float _diffY = 1.65f;

  public void SetLevels() {
    Positions();
  }

  void Positions() {
    HideAll();

    Vector3 startPosition = new Vector3(-2.42f, 2.50f, 0);

    int row = 0;

    GameCompany.Company company = PlayerManager.Instance.company.GetActualCompany();

    bool isWave = true;

    for (int i = 0; i < company.levels.Count; i++) {

      if (i != 0 && i % 4 == 0) row++;

      LevelsWievElement lwe = GetInstance();
      lwe.transform.localPosition = startPosition + new Vector3((i % 4) * _diffX, row * -_diffY, 0);
      lwe.gameObject.SetActive(true);
      lwe.SetWaves(false);

      GameCompany.Level level = new Level();

      if (company.levels.Count > i) {
        level = company.levels[i];
      } else {
        level = GameCompany.Level.CreateInstance();
      }

      lwe.SetInfo(i, level);

      lwe.OnSelect = () => {

        if (!PlayerManager.Instance.noAds
        && GoogleAdsMobile.Instance.NewLevelInterestionReady()) {


          GoogleAdsMobile.Instance.ShowInterstitialVideo(
            complete: (result) => {
              PlayerManager.Instance.company.actualLevelNum = lwe.num;
              GameManager.Instance.PlayGame();
            }
          );

        } else {
          PlayerManager.Instance.company.actualLevelNum = lwe.num;
          GameManager.Instance.PlayGame();
        }


      };
      levelsList.Add(lwe);

      if (isWave && !lwe.isOpen) {
        levelsList[levelsList.Count - 1].SetWaves(true);
        isWave = false;
      }

    }
    //levelsList[levelsList.Count - 1].SetWaves(true);
  }

  public LevelsWievElement GetInstance() {
    LevelsWievElement lwe = instLevelsList.Find(x => !x.gameObject.activeInHierarchy);

    if (lwe == null) {
      GameObject inst = Instantiate(elementPrefab);
      inst.transform.SetParent(transform);
      lwe = inst.GetComponent<LevelsWievElement>();
      instLevelsList.Add(lwe);
    }

    return lwe;
  }

  public void HideAll() {
    instLevelsList.ForEach(x => x.gameObject.SetActive(false));
    levelsList.Clear();
  }


  public ParticleSystem salut;
  public void PlaySalut() {
    salut.Play();
  }

  public void PauseSalut() {
    salut.Pause();
  }

  public void StopSalut() {
    salut.Stop();
  }
}
