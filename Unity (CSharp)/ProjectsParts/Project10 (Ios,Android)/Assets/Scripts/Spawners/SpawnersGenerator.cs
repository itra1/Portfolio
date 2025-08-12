using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersGenerator: Singleton<SpawnersGenerator> {

  private List<ResourceData> resourceData = new List<ResourceData>();
  private List<GameObject> instanceList = new List<GameObject>();

  protected override void Awake() {
    base.Awake();

    if (GameManager.activeLevelData == null) return;

    switch (GameManager.activeLevelData.gameMode) {
      case GameMode.survival:
        SurvivalGenerate();
        break;
      case GameMode.levelsClassic:
        ClassicCompanyGenerate();
        break;
      case GameMode.levelsConstructor:
        LevelConstructorGenerate();
        break;
    }

    

  }

  private void SurvivalGenerate() {
    // Стартовый дом
    CreateSpawnerObject("Prefabs/Decorations/StartSurvival");
    // Подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/HandingPlatform/HandingPlatform");
    // Монеты
    CreateSpawnerObject("Prefabs/Spawners/Coins/CoinsSpawner");
    // Пауки
    CreateSpawnerObject("Prefabs/Spawners/Spiders/Spiders");
    // Петы
    CreateSpawnerObject("Prefabs/Spawners/Pets/Pets");
    // Ящики
    CreateSpawnerObject("Prefabs/Spawners/Box/Box");
    // Вруги
    CreateSpawnerObject("Prefabs/Spawners/Enemy/Enemy");
    // Магия
    CreateSpawnerObject("Prefabs/Spawners/Magic/Magic");
    // Барьеры
    CreateSpawnerObject("Prefabs/Spawners/Barrier/BarrierSurvivle");
    // Петы
    CreateSpawnerObject("Prefabs/Spawners/Pets/PetSurvivle");

    // Декорации

    // Регионы
    CreateSpawnerObject("Prefabs/Spawners/Decor/Regions/RegionsSurvival");
    // Декорации Задника
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorBack/DecorBack");
    // Декорации за дорогой
    //CreateSpawnerObject("Prefabs/Spawners/Decor/DecorAfterRoad/DecorAfterRoad");
    // Декорации фронтальный декор
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFront/DecorFront");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFrontParticle/DecorFrontParticle");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorOnRoadParticle/DecorOnRoadParticle");
    // Декорации платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/PlatformsSpawner/PlatformsSurvivalSpawner");
    //CreateSpawnerObject("Prefabs/Spawners/Decor/PlatformsSpawner/PlatformsSpawner");
    // Декорации подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/TopPlatforms/TopPlatforms");
    //Декорации стены
    CreateSpawnerObject("Prefabs/Spawners/Decor/WallsSpawner/WallSurvivalSpawn");
  }

  private void ClassicCompanyGenerate() {
    // Стартовый дом
    CreateSpawnerObject("Prefabs/Decorations/StartLevel");
    // Подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/HandingPlatform/HandingPlatform");
    // Монеты
    CreateSpawnerObject("Prefabs/Spawners/Coins/CoinsSpawner");
    // Пауки
    CreateSpawnerObject("Prefabs/Spawners/Spiders/Spiders");
    // Петы
    CreateSpawnerObject("Prefabs/Spawners/Pets/Pets");
    // Ящики
    CreateSpawnerObject("Prefabs/Spawners/Box/Box");
    // Вруги
    CreateSpawnerObject("Prefabs/Spawners/Enemy/Enemy");
    // Магия
    CreateSpawnerObject("Prefabs/Spawners/Magic/Magic");
    // Барьеры
    CreateSpawnerObject("Prefabs/Spawners/Barrier/BarrierLevel");
    // Петы
    CreateSpawnerObject("Prefabs/Spawners/Pets/PetLevels");

    // Декорации

    // Регионы
    CreateSpawnerObject("Prefabs/Spawners/Decor/Regions/RegionsLevel");
    // Декорации Задника
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorBack/DecorBack");
    // Декорации за дорогой
    //CreateSpawnerObject("Prefabs/Spawners/Decor/DecorAfterRoad/DecorAfterRoad");
    // Декорации фронтальный декор
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFront/DecorFront");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFrontParticle/DecorFrontParticle");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorOnRoadParticle/DecorOnRoadParticle");
    // Декорации платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/PlatformsSpawner/PlatformsSpawner");
    // Декорации подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/TopPlatforms/TopPlatforms");
    // Декорации стены
    //CreateSpawnerObject("Prefabs/Spawners/Decor/WallsSpawner/WallsSpawnerLevels");
    CreateSpawnerObject("Prefabs/Spawners/Decor/WallsSpawner/WallsSpawnerSurvival");

  }

  private void LevelConstructorGenerate() {
    // Стартовый дом
    CreateSpawnerObject("Prefabs/Decorations/StartLevel");
    // Подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/HandingPlatform/HandingPlatform");
    // Подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/LevelBlockSpawner");

    // Декорации

    // Регионы
    CreateSpawnerObject("Prefabs/Spawners/Decor/Regions/RegionsLevel");
    // Декорации Задника
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorBack/DecorBack");
    // Декорации за дорогой
    //CreateSpawnerObject("Prefabs/Spawners/Decor/DecorAfterRoad/DecorAfterRoad");
    // Декорации фронтальный декор
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFront/DecorFront");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorFrontParticle/DecorFrontParticle");
    // Декорации фронтальные партиклы
    CreateSpawnerObject("Prefabs/Spawners/Decor/DecorOnRoadParticle/DecorOnRoadParticle");
    // Декорации подвесные платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/TopPlatforms/TopPlatforms");
    // Декорации стены
    CreateSpawnerObject("Prefabs/Spawners/Decor/WallsSpawner/WallsSpawner");
    // Декорации платформы
    CreateSpawnerObject("Prefabs/Spawners/Decor/PlatformsSpawner/PlatformConstructorSpawn");
  }

  private T CreateSpawnerObject<T>(string path) where T : MonoBehaviour {
    GameObject prefab = (GameObject)Resources.Load<GameObject>(path);
    GameObject instant = Instantiate(prefab);
    instant.SetActive(true);
    instant.transform.SetParent(transform);
    instant.transform.localPosition = prefab.transform.position;
    instant.transform.localScale = prefab.transform.localScale;
    return instant.GetComponent<T>();
  }
  private void CreateSpawnerObject(string path) {
    GameObject prefab = GetPrefab(path);
    //GameObject prefab = (GameObject)Resources.Load<GameObject>(path);
    GameObject instant = Instantiate(prefab);
    instant.SetActive(true);
    instant.transform.SetParent(transform);
    instant.transform.localPosition = prefab.transform.position;
    instant.transform.localScale = prefab.transform.localScale;
    instanceList.Add(instant);
  }

  private GameObject GetPrefab(string path) {

    var res = resourceData.Find(r => r.path == path);

    if(res != null)
      return res.prefab;

    GameObject prefab = (GameObject)Resources.Load<GameObject>(path);

    if(prefab == null)
      throw new System.Exception("No exists resource");

    resourceData.Add(new ResourceData() {
      path = path,
      prefab = prefab
    });

    return prefab;

  }


  public class ResourceData {
    public string path;
    public GameObject prefab;
  }

}
