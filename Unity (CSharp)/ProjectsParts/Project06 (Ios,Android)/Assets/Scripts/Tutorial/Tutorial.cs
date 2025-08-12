using UnityEngine;
using Game.User;

/// <summary>
/// Обучалка
/// </summary>
public class Tutorial : MonoBehaviour {

  public GameObject catDialogPrefab;
  [HideInInspector]
  public GameObject catDialogInstance;

  private void Start()
  {

  }



  void OnBattleStart() {

    if(UserManager.Instance.ActiveBattleInfo.Group == 1 && UserManager.Instance.ActiveBattleInfo.Level == 1 && UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company) {
      StartTutor_1_1();
    }else if(UserManager.Instance.ActiveBattleInfo.Group == 1 && UserManager.Instance.ActiveBattleInfo.Level == 2 && UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company) {
      StartTutor_1_2();
    }else if(UserManager.Instance.ActiveBattleInfo.Group == 1 && UserManager.Instance.ActiveBattleInfo.Level == 3 && UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company) {
      StartTutor_1_3();
    } else if(UserManager.Instance.ActiveBattleInfo.Group == 1 && UserManager.Instance.ActiveBattleInfo.Level == 4 && UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company) {
      StartTutor_1_4();
    }
  }

  void OnCreateBullet(Game.Weapon.Bullet bullet) {
    if(isMission_1_1 == 5 && spawnBulletInst == null) {
      spawnBulletInst = bullet;
      spawnBulletInst.isDamage = false;
    }
  }

  GameObject spawnEnemy;
  Game.Weapon.Bullet spawnBulletInst;

  void OnCreateEnemy(GameObject enemyInst) {
    if(isMission_1_1 == 1) {
      EnemysSpawn.OnSpawnEnemy -= OnCreateEnemy;
      spawnEnemy = enemyInst;
    }else if(isMission_1_2 == 1) {
      if(enemyInst.GetComponent<Enemy>().enemyType == EnemyType.Ekstrimalchik) {
        EnemysSpawn.OnSpawnEnemy -= OnCreateEnemy;
        spawnEnemy = enemyInst;
      }
    } else if(isMission_1_3 == 1) {
      if(enemyInst.GetComponent<Enemy>().enemyType == EnemyType.Makdaun) {
        EnemysSpawn.OnSpawnEnemy -= OnCreateEnemy;
        spawnEnemy = enemyInst;
      }
    } else if(isMission_1_4 == 1) {
      if(enemyInst.GetComponent<Enemy>().enemyType == EnemyType.Hunweibin) {
        EnemysSpawn.OnSpawnEnemy -= OnCreateEnemy;
        spawnEnemy = enemyInst;
      }
    }
  }

  public void CreateDialog() {
    catDialogInstance = Instantiate(catDialogPrefab);
    catDialogInstance.GetComponent<RectTransform>().SetParent(UIController.Instance.transform);
    catDialogInstance.transform.localScale = Vector3.one;
    catDialogInstance.transform.localPosition = Vector3.zero;
  }

  #region Обучение миссии 1-1

  int isMission_1_1 = 0;

  string mission_1_1_1 = "Что это за урод? Что ему надо у моего дома?";
  string mission_1_1_2 = "Запущу-ка я в него помидором, что бы не шарился где попало!";
  string mission_1_1_3 = "Ну-ка, нажми сюда!";
  string mission_1_1_4 = "Что, тебе мало? Ну получи еще!";

  void StartTutor_1_1() {
    if(isMission_1_1 > 0) return;
    isMission_1_1 = 1;
    spawnBulletInst = null;

    EnemysSpawn.OnSpawnEnemy += OnCreateEnemy;
  }

  void Tutor_1_1_Update() {
    if(isMission_1_1 == 1) {
      if(spawnEnemy != null && spawnEnemy.transform.position.x <= CameraController.rightPoint.x-2) {
        isMission_1_1 = 2;

        Time.timeScale = 0;
        CreateDialog();
        catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_1_1;
        catDialogInstance.GetComponent<CatDialog>().OnTapDisplay = Tutor_1_OnTapDisplay;
        catDialogInstance.GetComponent<CatDialog>().OnTapRegion = Tutor_1_OnTapRegion;
        catDialogInstance.SetActive(true);
      }
    }else if(isMission_1_1 == 5) {

      if(spawnBulletInst == null || (spawnBulletInst != null && spawnBulletInst.velocity.y < 0 && spawnEnemy.transform.position.y + 1 >= spawnBulletInst.transform.position.y)) {
        spawnBulletInst.DamageEnemy(spawnEnemy, 0);
      }
    }
  }

  void Tutor_1_OnTapDisplay() {

    if(isMission_1_1 == 2) {
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_1_2;
      isMission_1_1 = 3;
    }else if(isMission_1_1 == 3) {
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_1_3;
      isMission_1_1 = 4;
      catDialogInstance.GetComponent<CatDialog>().regionObject.transform.position = new Vector3(spawnEnemy.transform.position.x-2.2f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().regionObject.SetActive(true);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.transform.position = new Vector3(spawnEnemy.transform.position.x - 2.2f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.SetActive(true);
    }

  }
  void Tutor_1_OnTapRegion() {
    if(isMission_1_1 == 4) {
      Time.timeScale = 1;
      RectTransform reg = catDialogInstance.GetComponent<CatDialog>().regionObject.GetComponent<RectTransform>();
      isMission_1_1 = 5;
      PlayerController.Instance.OnDisplayTap(PlayerController.Instance.transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth/2 + reg.anchoredPosition.x, Camera.main.pixelHeight / 2 + reg.anchoredPosition.y, 0)));
      catDialogInstance.GetComponent<CatDialog>().regionObject.SetActive(false);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.SetActive(false);
      catDialogInstance.GetComponent<CatDialog>().dialogObject.SetActive(false);
      spawnEnemy.GetComponent<Enemy>().OnDamageEvnt += OnDamageEvnt1_1;
    } else if(isMission_1_1 == 6) {
      isMission_1_1 = 7;
      Time.timeScale = 1;
      RectTransform reg = catDialogInstance.GetComponent<CatDialog>().regionObject.GetComponent<RectTransform>();
      PlayerController.Instance.OnDisplayTap(PlayerController.Instance.transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2 + reg.anchoredPosition.x, Camera.main.pixelHeight / 2 + reg.anchoredPosition.y, 0)));
      Destroy(catDialogInstance);
    }
  }

  void OnDamageEvnt1_1(Enemy enemyDamage, GameObject damager, float value) {

    if(isMission_1_1 == 5) {
      Time.timeScale = 0;
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_1_4;
      catDialogInstance.GetComponent<CatDialog>().regionObject.transform.position = new Vector3(spawnEnemy.transform.position.x - 1.9f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().regionObject.SetActive(true);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.transform.position = new Vector3(spawnEnemy.transform.position.x - 1.9f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.SetActive(true);
      catDialogInstance.GetComponent<CatDialog>().dialogObject.SetActive(true);
      isMission_1_1 = 6;
    }

  }

  #endregion

  #region Обучение миссии 1-2

  int isMission_1_2 = 0;
  string mission_1_2_1 = "А эти пошустрее!";
  string mission_1_2_2 = "Надо бить с упреждением!";

  void StartTutor_1_2() {
    if(isMission_1_2 > 0) return;
    isMission_1_2 = 1;

    EnemysSpawn.OnSpawnEnemy += OnCreateEnemy;
  }

  void Tutor_1_2_Update() {
    if(isMission_1_2 == 1) {
      if(spawnEnemy != null && spawnEnemy.transform.position.x <= CameraController.rightPoint.x - 2) {
        isMission_1_2 = 2;

        Time.timeScale = 0;
        CreateDialog();
        catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_2_1;
        catDialogInstance.GetComponent<CatDialog>().OnTapDisplay = Tutor_2_OnTapDisplay;
        catDialogInstance.GetComponent<CatDialog>().OnTapRegion = Tutor_2_OnTapRegion;
        catDialogInstance.SetActive(true);
      }
    }
  }

  void Tutor_2_OnTapDisplay() {
    if(isMission_1_2 == 2) {
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_2_2;
      isMission_1_2 = 3;
      catDialogInstance.GetComponent<CatDialog>().regionObject.transform.position = new Vector3(spawnEnemy.transform.position.x - 3.5f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().regionObject.SetActive(true);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.transform.position = new Vector3(spawnEnemy.transform.position.x - 3.5f, spawnEnemy.transform.position.y, 0);
      catDialogInstance.GetComponent<CatDialog>().pointerInstance.SetActive(true);
      catDialogInstance.GetComponent<CatDialog>().dialogObject.SetActive(true);
    }
  }

  void Tutor_2_OnTapRegion() {
    if(isMission_1_2 == 3) {
      isMission_1_2 = 4;
      Time.timeScale = 1;
      RectTransform reg = catDialogInstance.GetComponent<CatDialog>().regionObject.GetComponent<RectTransform>();
      PlayerController.Instance.OnDisplayTap(PlayerController.Instance.transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2 + reg.anchoredPosition.x, Camera.main.pixelHeight / 2 + reg.anchoredPosition.y, 0)));
      catDialogInstance.SetActive(false);
    }
  }

  void OnDamageEvnt1_2(Enemy enemyDamage, float value) {
  }

  #endregion

  #region Обучение миссии 1-3

  int isMission_1_3 = 0;
  string mission_1_3_1 = "А эти пожирнее, отъелись на баг-маках!";
  string mission_1_3_2 = "Ну ничего, помидор у меня на всех хватит!";

  void StartTutor_1_3() {
    if(isMission_1_3 > 0) return;
    isMission_1_3 = 1;

    EnemysSpawn.OnSpawnEnemy += OnCreateEnemy;
  }

  void Tutor_1_3_Update() {
    if(isMission_1_3 == 1) {
      if(spawnEnemy != null && spawnEnemy.transform.position.x <= CameraController.rightPoint.x - 2) {
        isMission_1_3 = 2;

        Time.timeScale = 0;
        CreateDialog();
        catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_3_1;
        catDialogInstance.GetComponent<CatDialog>().OnTapDisplay = Tutor_3_OnTapDisplay;
        catDialogInstance.GetComponent<CatDialog>().OnTapRegion = Tutor_3_OnTapRegion;
        catDialogInstance.SetActive(true);
      }
    }
  }

  void Tutor_3_OnTapDisplay() {
    if(isMission_1_3 == 2) {
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_3_2;
      isMission_1_3 = 3;
    }else if(isMission_1_3 == 3) {
      isMission_1_3 = 4;
      Time.timeScale = 1;
      catDialogInstance.SetActive(false);
    }
  }

  void Tutor_3_OnTapRegion() {
  }

  #endregion

  #region Обучение миссии 1-4

  int isMission_1_4 = 0;
  string mission_1_4_1 = "Эти с автоматами! Опасные типчики!";
  string mission_1_4_2 = "Надо бы их обстреливать в первую очередь!";

  void StartTutor_1_4() {
    if(isMission_1_4 > 0) return;
    isMission_1_4 = 1;

    EnemysSpawn.OnSpawnEnemy += OnCreateEnemy;
  }

  void Tutor_1_4_Update() {
    if(isMission_1_4 == 1) {
      if(spawnEnemy != null && spawnEnemy.transform.position.x <= CameraController.rightPoint.x - 2) {
        isMission_1_4 = 2;

        Time.timeScale = 0;
        CreateDialog();
        catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_4_1;
        catDialogInstance.GetComponent<CatDialog>().OnTapDisplay = Tutor_4_OnTapDisplay;
        catDialogInstance.GetComponent<CatDialog>().OnTapRegion = Tutor_4_OnTapRegion;
        catDialogInstance.SetActive(true);
      }
    }
  }

  void Tutor_4_OnTapDisplay() {
    if(isMission_1_4 == 2) {
      catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_4_2;
      isMission_1_4 = 3;
    } else if(isMission_1_4 == 3) {
      isMission_1_4 = 4;
      Time.timeScale = 1;
      catDialogInstance.SetActive(false);
    }
  }

  void Tutor_4_OnTapRegion() {
  }

  #endregion

  #region Прокачка

  //int isProgress = 0;
  //string progress_1 = "Давай посмотрим, на что я могу потратить эти деньги!";
  //string progress_2 = "Давай-ка прокачаем Силушку, чтоб сильнее гадов заморских бить!";
  //string progress_3 = "Ну а теперь можно и обратно в бой!";

  //void OnStartMap() {

  //  if(isMission_1_1 > 0) return;
  //  isMission_1_1 = 1;

  //  if(User.instance.coins > 1000) {
  //    CreateDialog();
  //    catDialogInstance.GetComponent<CatDialog>().textInfo.text = mission_1_1_1;
  //    catDialogInstance.GetComponent<CatDialog>().OnTapDisplay = Tutor_1_OnTapDisplay;
  //    catDialogInstance.GetComponent<CatDialog>().OnTapRegion = Tutor_1_OnTapRegion;
  //    catDialogInstance.SetActive(true);
  //  }

  //}

  #endregion


}
