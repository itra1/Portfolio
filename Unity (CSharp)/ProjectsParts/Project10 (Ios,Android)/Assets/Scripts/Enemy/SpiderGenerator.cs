using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Генератор пауков
/// </summary>
public class SpiderGenerator : MonoBehaviour {

  public const string POOLER_KEY = "SpiderGenerator";

  public static SpiderGenerator instance;
  public GameObject coinsCountObject;
  public Player.Jack.PlayerMove player;

  public GameObject[] spiders; //крысы

  // Время между генерациями групп
  public float timeGenerateMin;
  public float timeGenerateMax;

  // Число генерируемое в группе
  public int generateCountMin;
  public int generateCountMax;

  private float nextTimeGenerator; // Время следующей генерации
  private bool generate = false; // Разрешение генерации

  private float itemTime;
  private bool itemGenerate = true;
  private int itemCount;
  private int itemThis;

  List<KeyValuePair<GameObject , int>> spiderList;
  List<float> poolCd;

  RunnerPhase runnerPhase;

  [HideInInspector]
  bool _spiderDamage = true;                  // Флаг, что игрок может убить паука, что бы игрок мог убивать только 1 паука за раз
  public static bool spiderDamage {
    get { return instance._spiderDamage; }
  }

  void Start() {
    instance = this;

    RunnerController.OnChangeRunnerPhase += ChangePhase;
    Player.Jack.PlayerMove.OnPlayerGround += PlayerGround;
    RunSpawner.OnGenerateMapObject += OnGenerateMapObject;
    //runner = gameController.GetComponent<RunnerController>();
    itemTime = Time.time;

    StructProcessor();
    LevelPooler.Instance.AddPool(POOLER_KEY, spiderList);

  }

  void OnDestroy() {
    Player.Jack.PlayerMove.OnPlayerGround -= PlayerGround;
    RunnerController.OnChangeRunnerPhase -= ChangePhase;
    RunSpawner.OnGenerateMapObject -= OnGenerateMapObject;
  }

  void Update() {
    if(runnerPhase != RunnerPhase.run) return;             // Запрет генерации не в стадии забега
    if(RunnerController.playerDistantion <= 50) return;        // Запрет генерации ранее 50 метров со старта

    if(GameManager.activeLevelData.gameMode == GameMode.survival) RunGenerator();

  }

  void RunGenerator() {
    // Рассчитываем будущую вероятнось
    if(!generate && RunnerController.Instance.generateItems) {
      nextTimeGenerator = Time.time + Random.Range(timeGenerateMin, timeGenerateMax);
      itemCount = (int)Mathf.Round(Random.Range(generateCountMin, generateCountMax));
      itemTime = Time.time;
      itemGenerate = true;
      itemThis = 0;
      generate = true;
    }

    // генерируем
    if(generate && Time.time >= nextTimeGenerator && RunnerController.Instance.generateItems) {
      if(!itemGenerate) {
        itemTime = Time.time + Random.Range(0.05f, 0.3f);
        itemGenerate = true;
      }

      if(itemGenerate && itemTime <= Time.time) {
        Spawn();
        itemThis++;
        itemGenerate = false;
      }

      if(itemThis >= itemCount) generate = false;
    }
  }

  void OnGenerateMapObject(MapObject generateObject) {
    if(generateObject == MapObject.spider) Spawn();
  }

  void PlayerGround() {
    if(!_spiderDamage)
      combo = 0;
  }

  void ChangePhase(RunnerPhase newPhase) {
    runnerPhase = newPhase;
  }

  void LateUpdate() {
    if(!_spiderDamage && player && player.velocity.y < 0) {
      _spiderDamage = true;
    }
  }

  void SpiderDead(Vector3 spiderPosition) {
    
    _spiderDamage = false;
    if(combo == 0)
      combo = 2;
    else
      combo *= 2;
    CoinsGenerator(combo, spiderPosition);
    RunnerController.spiderkill++;
    Questions.QuestionManager.ConfirmQuestion(Quest.crushSpiders, 1, spiderPosition);

    GenerateBlood(spiderPosition);
  }

  void Spawn() {
    GameObject obj = LevelPooler.Instance.GetPooledObject(POOLER_KEY, Random.Range(0 , spiders.Length));
    obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.transform.position.y, 0);
    obj.SetActive(true);
    obj.GetComponent<Spider>().OnDead = SpiderDead;
    obj.transform.parent = transform;
  }

  int combo = 0;

  void CoinsGenerator(int coinsCount, Vector3 position) {

		CoinsSpawner.GenOneMonetToPlayer(position, coinsCount);

    GameObject count = Instantiate(coinsCountObject , position + Vector3.right , Quaternion.identity) as GameObject;
    count.transform.parent = transform;
    count.GetComponent<TextMesh>().text = "x" + combo.ToString();
    count.GetComponent<MeshRenderer>().sortingLayerName = "SFX";
    count.GetComponent<MeshRenderer>().sortingOrder = 30;
    LightTween.SpriteColorTo(count.GetComponent<MeshRenderer>(), new Color(1, 1, 1, 0), 1);
    LightTween.MoveTo(count, count.transform.localPosition + Vector3.up, 1, 0, LightTween.EaseType.linear, false, null, null);
    Destroy(count, 1);

  }

  void StructProcessor() {
    spiderList = new List<KeyValuePair<GameObject, int>>();
    poolCd = new List<float>();

    float sum1 = 0;

    poolCd.Add(sum1);
    foreach(GameObject rat in spiders) {
      spiderList.Add(new KeyValuePair<GameObject, int>(rat, 5));
      sum1 += 1;
      poolCd.Add(sum1);
    }

    if(sum1 != 1f)
      poolCd = poolCd.ConvertAll(x => x / sum1);
  }

  public void GenerateBlood(Vector3 position) {
    GameObject bd = Pooler.GetPooledObject("SpederBlood");
    bd.transform.position = position;
    bd.SetActive(true);
    StartCoroutine(WaitDeactive(bd, 0.7f));
  }

  IEnumerator WaitDeactive(GameObject obj, float time) {
    yield return new WaitForSeconds(time);
    obj.SetActive(false);
  }

}
