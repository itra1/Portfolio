using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Road.Spawners.Pets {

  

  /// <summary>
  /// Генератор петов
  /// </summary>
  public abstract class PetSpawner: Singleton<PetSpawner> {

    protected RunnerPhase runnerPhase;
    
    protected virtual void Start() {
      Load();
    }

    public static int levelDino;
    public static int levelSpider;
    public static int levelBat;

    void Load() {
      levelBat = PlayerPrefs.GetInt("penBat");
      levelSpider = PlayerPrefs.GetInt("penSpider");
      levelDino = PlayerPrefs.GetInt("penDino");
    }

    /// <summary>
    /// возвращает информацию о уровне прокачки
    /// </summary>
    /// <param name="petType"></param>
    /// <returns></returns>
    public static int GetPetLevel(Player.Jack.PetsTypes petType) {
      switch (petType) {
        case Player.Jack.PetsTypes.bat:
          return levelBat;
        case Player.Jack.PetsTypes.spider:
          return levelSpider;
        default:
          return levelDino;
      }
    }

    public void ActivateParticles(Vector3 neepPosition) {
      GameObject particle = Pooler.GetPooledObject("PotsActivEffect");
      particle.transform.position = neepPosition;
      particle.SetActive(true);
      StartCoroutine(WaitAndDeactive(particle, 1));
    }
    IEnumerator WaitAndDeactive(GameObject obj, float time) {
      yield return new WaitForSeconds(time);
      obj.SetActive(false);
    }

  }

}