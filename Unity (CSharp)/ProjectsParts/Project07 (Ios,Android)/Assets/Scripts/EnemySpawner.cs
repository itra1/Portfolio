using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor: Editor {

  private int numFormation = 0;

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    EditorGUILayout.BeginHorizontal();
    numFormation = EditorGUILayout.IntField("Номер формации: ", numFormation);
    if (GUILayout.Button("Positing")) {

      ((EnemySpawner)target).PositionEditor(numFormation);

    }
    EditorGUILayout.EndHorizontal();

  }

}

#endif

public class EnemySpawner: Singleton<EnemySpawner> {

  public DropSpawner dropSpawner;

  [SerializeField]
  private List<Enemy> _instancesEnemys = new List<Enemy>();

  public Enemy enemyPrefab;

  public GameObject skullPrefab;
  private static Vector2 leftDown;
  private static Vector2 rightTop;

  private static float DistanseX {
    get { return rightTop.x - leftDown.x; }
  }

  private static float DistanseY {
    get { return rightTop.y - leftDown.y; }
  }
  public Vector3[] position;
  public Vector3[] scaling;

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleStart))]
  private void BattleStart(ExEvent.BattleEvents.BattleStart eventData)
  {

    EnemyManager.Instance.LoadEnemyGraphic(UserManager.Instance.ActiveLocation.Block);

    enemyList.ForEach(x => x.gameObject.SetActive(false));
    //instancesEnemys.Clear();

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.NextWave))]
  public void NextWave(ExEvent.BattleEvents.NextWave eventData) {
    if (eventData.isBoss) {
      GenerateBossEnemy();
    } else {
      GenerateWaveEnemy();
    }
  }

  private void Start() {

    Enemy.DeadEvent += DiedEnemy;
    Enemy.OnDamageEvent += OnDamageEvent;
    EnemyBoss.DeadBossEvent += DeadEnemyBoss;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    Enemy.DeadEvent -= DiedEnemy;
    Enemy.OnDamageEvent -= OnDamageEvent;
    EnemyBoss.DeadBossEvent -= DeadEnemyBoss;
  }

  private List<Enemy> enemyList = new List<Enemy>();

  private void GenerateWaveEnemy() {

    enemyList.Clear();

    EnemyManager.Formation enemForm = EnemyManager.Instance.GetRandomFormation();

    List<int> graphics = new List<int>();

    for (int i = 1; i < EnemyManager.Instance.GetEnemyGraphicCount(); i++)
    {
      graphics.Add(i);
    }

    enemForm.enemyPositions.ForEach(elem => {
      int indexEnemy = graphics[Random.Range(0, graphics.Count)];
      graphics.Remove(indexEnemy);
      Enemy enemy = GetInstance();
      enemy.SetData(elem
        , UserManager.Instance.ActiveLocation.GetEnemyHealth(BattleController.Instance.activeState)
        , UserManager.Instance.ActiveLocation.GetEnemyDamage(BattleController.Instance.activeState)
        , UserManager.Instance.ActiveLocation.GetEnemyReload(BattleController.Instance.activeState)
        );
      enemy.SetBodyGraphic(indexEnemy);
      enemyList.Add(enemy);
      enemy.StartDead = (en) => {
        dropSpawner.GenerateBonus(en);
        ShowDeadEffect(en);
      };

      enemy.gameObject.SetActive(true);
    });
    enemyList = enemyList.OrderBy(o => o.transform.position.y).ToList();

  }

  private Enemy GetInstance() {

    Enemy enemy = _instancesEnemys.Find(x => !x.gameObject.activeInHierarchy);

    if (enemy == null) {
      enemyPrefab.gameObject.SetActive(false);
      GameObject inst = Instantiate(enemyPrefab.gameObject);
      enemy = inst.GetComponent<Enemy>();
      _instancesEnemys.Add(enemy);
    }
    return enemy;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponShoot))]
  private void EnemyClick(ExEvent.BattleEvents.WeaponShoot eventData) {

    if (eventData.weapon.massDMG) {
      enemyList.ForEach(elem => elem.Damage(eventData.weapon.Damage));
      return;
    }

    if (eventData.enemy != null && eventData.enemy.isDead)
      eventData.enemy = null;

    Enemy damageEnemy = null;

    if (eventData.enemy != null) {
      damageEnemy = eventData.enemy;
    } else {
      if (enemyList.Count > 0)
        damageEnemy = enemyList.Find(x => !x.isDead);
    }

    if (damageEnemy != null)
      damageEnemy.Damage(eventData.weapon.Damage);

  }

  public void DieAllForceAll()
  {
    foreach (var enemy in enemyList)
    {
      if(!enemy.isDead)
        enemy.Damage(999999);
    }
  }

  private void ShowDeadEffect(Enemy enemy) {
    GameObject istScull = Instantiate(skullPrefab, enemy.transform.position, Quaternion.identity);
    istScull.transform.localScale = enemy.transform.localScale;
    Destroy(istScull, 1);
  }

  private void DiedEnemy(Enemy enemy) {
    enemyList.Remove(enemy);

    if (BattleController.Instance.isBoss)
      QuestionManager.Instance.AddValueQuest(QuestionManager.Type.killBoss, 1);
    else
      QuestionManager.Instance.AddValueQuest(QuestionManager.Type.killEnemy, 1);

    if (enemyList.Count <= 0)
      AllDeadEnemy();

  }

  private void OnDamageEvent(Enemy enemy) {
    if (!BattleController.Instance.isBoss)
      return;
    dropSpawner.GenerateBonus(enemy);

  }

  private void AllDeadEnemy() {
    BattleController.Instance.WaveComplete();
  }

  private void DeadEnemyBoss(EnemyBoss enemy) {
    //DeadBossEvent

  }

  private void GenerateBossEnemy() {
    Enemy enemy = GetInstance();
    enemy.SetData(EnemyManager.Instance.BossPosition
      , UserManager.Instance.ActiveLocation.GetEnemyHealth(BattleController.Instance.activeState)
      , UserManager.Instance.ActiveLocation.GetEnemyDamage(BattleController.Instance.activeState)
      , UserManager.Instance.ActiveLocation.GetEnemyReload(BattleController.Instance.activeState)
      );
    enemyList.Add(enemy);
    enemy.SetBodyGraphic(0);
    enemy.gameObject.SetActive(true);
  }

  public void PositionEditor(int numFormation) {
    EnemyManager.Formation enemForm = EnemyManager.Instance.GetFormation(numFormation);

    enemForm.enemyPositions.ForEach(elem => {
      Enemy enemy = GetInstance();
      enemy.transform.position = elem.position;
      enemy.transform.localScale = elem.scaling;
      enemy.gameObject.SetActive(true);
    });

  }

}
