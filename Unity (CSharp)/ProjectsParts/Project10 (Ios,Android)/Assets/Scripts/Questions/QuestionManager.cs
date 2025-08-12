using UnityEngine;
using System.Collections.Generic;
using MiniJSON;
using Questions;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(Questions.QuestionManager))]
public class QuestionManagerEditor: Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    //if (GUILayout.Button("CopyList")) {

    //	QuestionManager script = (QuestionManager)target;

    //	script.companyList.Clear();

    //	for (int i = 0; i < script.allQuestions.Length; i++)
    //		script.companyList.Add(new LevelList() {
    //			isComplete = script.allQuestions[i].listComplited,
    //			items = script.allQuestions[i].items
    //		});

    //}

  }
}

#endif

namespace Questions {

  public class SaveData {

    public SaveData() {
      elementList = new List<Element>();
    }

    public List<Element> elementList;

    public class Element {
      public bool isCpmplete;
      public List<float> values = new List<float>();
    }

  }


  public class QuestionManager: Singleton<QuestionManager> {

    public List<DescriptionLib> descriptions;
    public LevelList[] companyList;
    public LevelList[] shipList;

    //public OneListQuestions[] allQuestions;
    //public OneListQuestions[] alterQuestions;

    private LevelList _actionList;

    public LevelList actionList {
      set { _actionList = value; }
      get { return _actionList; }
    }

    private List<WeaponTypes> weaponList = new List<WeaponTypes>();

    private void Start() {

      actionList = new LevelList();

      FillAllQuestions();
      InitQuestionsGroup();

#if UNITY_EDITOR
      //TestHaveQuest();
#endif
      CheckKeysTrans();
    }

    protected override void OnDestroy() {
      base.OnDestroy();
      UnSubscribeEvent();
    }

    public void FillAllQuestions(bool fillActValue = false) {

      //ConfigData configData = Config.instant.configData.GetConfigData("levels");
      companyList = new LevelList[Config.Instance.config.levels.Count];
      FillQuestList(ref companyList, Config.Instance.config.levels);
      Load();

      //configData = Config.instant.configData.GetConfigData("alterQuest");
      //listLevels = (List<object>)configData.config;
      //alterQuestions = new OneListQuestions[listLevels.Count];
      //FillQuestList(ref alterQuestions, listLevels);
      //LoadDataAlterQuest();

    }

    void OnApplicationQuit() {
      Save();
    }

    void FillQuestList(ref LevelList[] questArr, List<Configuration.Levels.Level> listLevels) {
      for (int level = 0; level < listLevels.Count; level++) {
        List<Configuration.Levels.Quest> questList = listLevels[level].questions;
        questArr[level].items = new Question[questList.Count];
        for (int quest = 0; quest < questList.Count; quest++) {
          questArr[level].items[quest].type = descriptions.Find(x => x.name == questList[quest].name).type;
          questArr[level].items[quest].descr = questList[quest].name;
          questArr[level].items[quest].faild = false;
          questArr[level].items[quest].cumulative = false;
          questArr[level].items[quest].needvalue = questList[quest].count;
          //questArr[level].items[quest].price = int.Parse(oneQuest["price"].ToString());

          //if(oneQuest.ContainsKey("valAllRun")) {
          //  questArr[level].items[quest].needvalue = float.Parse(oneQuest["valAllRun"].ToString());
          //  questArr[level].items[quest].cumulative = true;
          //} else
          //  questArr[level].items[quest].needvalue = float.Parse(oneQuest["valOneRun"].ToString());
        }
      }
    }

    void Save() {
      PlayerPrefs.SetString("shipQuestions", SaveDataQuest(shipList));
      PlayerPrefs.SetString("classicQuestions", SaveDataQuest(companyList));
    }

    string SaveDataQuest(LevelList[] questArray) {

      SaveData save = new SaveData();

      for (int i = 0; i < questArray.Length; i++) {

        SaveData.Element elem = new SaveData.Element();
        elem.isCpmplete = questArray[i].isComplete;

        for (int j = 0; j < questArray[i].items.Length; j++)
          elem.values.Add(questArray[i].items[j].cumulative ? questArray[i].items[j].value : 0);

        save.elementList.Add(elem);
      }
      return Newtonsoft.Json.JsonConvert.SerializeObject(save);

    }

    void Load() {

      if (!PlayerPrefs.HasKey("classicQuestions")) return;
      LoadDataQuest(ref companyList, Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("classicQuestions")));

    }

    void LoadDataAlterQuest() {

      if (!PlayerPrefs.HasKey("shipQuestions")) return;
      LoadDataQuest(ref shipList, Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("shipQuestions")));

    }

    void LoadDataQuest(ref LevelList[] questArray, SaveData saveData) {

      for (int i = 0; i < questArray.Length; i++) {
        questArray[i].isComplete = saveData.elementList[i].isCpmplete;

        for (int j = 0; j < questArray[i].items.Length; j++) {
          if (saveData.elementList[i].values.Count > j)
            questArray[i].items[j].value = saveData.elementList[i].values[j];
          else
            questArray[i].items[j].value = 0;
        }
      }

    }

    public bool ExistsActiveQuests() {
      return (actionList.items != null && actionList.items.Length > 0 ? true : false);
    }

    /// <summary>
    /// Для друзей с дальнего востока, в частности Китая, сделаем послабление по квестам из ФБ
    /// </summary>
    void CheckChine() {

      if (Application.systemLanguage == SystemLanguage.Chinese) {
        for (int i = 0; i < actionList.items.Length; i++) {
          if ((actionList.items[i].type == Quest.invateFriends
              || actionList.items[i].type == Quest.shareGame
              || actionList.items[i].type == Quest.contactFb)
              && actionList.items[i].needvalue > Instance.actionList.items[i].value
              ) {
            actionList.items[i].value = actionList.items[i].needvalue;
            CheckQuestions(i);
          }
        }
      }

    }

    public static Question GetQuestValue(Quest needType) {
      Question questValue = new Question();
      for (int i = 0; i < Instance.actionList.items.Length; i++)
        if (Instance.actionList.items[i].type == needType)
          questValue = Instance.actionList.items[i];
      return questValue;
    }

    public static bool CheckActionQuest(Quest needType) {
      if (Instance == null || Instance.actionList.items == null) return false;
      for (int i = 0; i < Instance.actionList.items.Length; i++)
        if (Instance.actionList.items[i].type == needType)
          return true;

      return false;
    }

    public void InitQuestionsGroup() {

      actionList = new LevelList();

      if (!GameManager.isAlterQuest) {

        if (GameManager.activeLevel < GameManager.level) return;
        UnSubscribeEvent();

        if (!GameManager.Instance.isDebug)
          if (getStatusComplList(GameManager.activeLevel)) return;
        actionList = companyList[GameManager.activeLevel];
      } else {
        Debug.Log(shipList.Length);
        actionList = shipList[GameManager.activeLevel];
      }

      for (int j = 0; j < actionList.items.Length; j++) {
        if (!actionList.items[j].cumulative)
          actionList.items[j].value = getStatusVallItem(j);
        actionList.items[j].descr = actionList.items[j].descr.Replace("(n)", actionList.items[j].needvalue.ToString());
        actionList.items[j].faild = false;
        SubscribeNeedsEvent(actionList.items[j].type);
      }

      try {
        if (FBController.CheckFbLogin) fbContact();
      } catch {
        Debug.Log("Question error: no contact FB");
      }

      CheckChine();

      //if(GameManager.instance.isDebug) {
      //  actionList = allQuestions[GameManager.activeLevel];
      //  for(int j = 0; j < actionList.items.Length; j++) {
      //    actionList.items[j].value = getStatusVallItem(j);
      //    actionList.items[j].descr = actionList.items[j].descr.Replace("(n)", actionList.items[j].needvalue.ToString());
      //    actionList.items[j].faild = false;
      //    SubscribeNeedsEvent(actionList.items[j].type);
      //  }
      //} else {
      //  for(int i = 0; i < allQuestions.Length; i++) {
      //    if(!getStatusComplList(i)) {
      //      numList = i;
      //      actionList = allQuestions[i];
      //      for(int j = 0; j < actionList.items.Length; j++) {
      //        actionList.items[j].value = getStatusVallItem(j);
      //        actionList.items[j].descr = actionList.items[j].descr.Replace("(n)", actionList.items[j].needvalue.ToString());
      //        actionList.items[j].faild = false;
      //        SubscribeNeedsEvent(actionList.items[j].type);
      //      }

      //      // Попытка выполнить квест с авторизаций
      //      try {
      //        if(FBController.CheckFbLogin) fbContact();
      //      } catch {
      //        Debug.Log("Question error: no contact FB");
      //      }

      //      CheckChine();

      //      return;
      //    }
      //  }
      //}
    }

    public static LevelList GetQuestListLevel(int level, bool alterQuest = false) {
      if (alterQuest)
        return Instance.shipList[level];
      else
        return Instance.companyList[level];
    }

    // Обновляем список при необходимости
    public void UpdateList() {

      if (CheckList())
        InitQuestionsGroup();
    }

    // Проверка готовности текущего списка
    public bool CheckList() {

      bool updateList = true;

      foreach (Question item in actionList.items) {
        if (item.needvalue > item.value)
          updateList = false;
      }

      return updateList;
    }

    // Получаем отметку о готовности списка
    bool getStatusComplList(int num) {
      string saveParam = "quastion" + "List" + num;

      return PlayerPrefs.GetInt(saveParam) == 1 ? true : false;
    }

    public static void ClearAllQuestionComplited() {
      for (int i = 0; i < 101; i++) {
        PlayerPrefs.DeleteKey("quastion" + "List" + i);
      }
      for (int i = 0; i < 4; i++) {
        PlayerPrefs.DeleteKey("quastion" + "Item" + i);
      }
    }

    // Сохраняем значение готовности списка
    void saveStatusSomplList(int num, bool flag) {
      string saveParam = "quastion" + "List" + num;
      PlayerPrefs.SetInt(saveParam, (flag == true ? 1 : 0));

      // Если мы закончили список, обнуляем сохраненные значения
      if (flag) {
        for (int i = 0; i <= companyList[num].items.Length; i++)
          saveStatusVallItem(i, 0);
      }
    }

    // Получем значение о готовности элемента списка
    float getStatusVallItem(int num) {
      string saveParam = "quastion" + "Item" + num;
      return PlayerPrefs.GetFloat(saveParam);
    }

    // Сохраняем значение о готовности элемента списка
    void saveStatusVallItem(int num, float flag) {
      string saveParam = "quastion" + "Item" + num;
      PlayerPrefs.SetFloat(saveParam, flag);
    }
    // Сохраняем значение квеста
    void CheckQuestions(int num) {

      if (GameManager.activeLevelData.gameMode == GameMode.survival) return;

      if (actionList.items[num].needvalue <= actionList.items[num].value) {
        ExEvent.GameEvents.QuestionComplete.CallAsync(actionList.items[num]);
        //RunnerGamePlay.QuestComplited(actionList.items[num]);
        saveStatusVallItem(num, actionList.items[num].needvalue);
        //AppMetrica.Instance.ReportEvent("Квесты: выполнено " + Lanuage.GetTranslate(actionList.items[num].descr));

        RunnerController run = RunnerController.Instance;
        if (run != null) {
          run.questionInRaceList.Add(num);
          UnSubscribeEvent(actionList.items[num].type);
        }

        CheckFullQuest();

      } else if (actionList.items[num].cumulative)
        saveStatusVallItem(num, actionList.items[num].value);
      else if (actionList.items[num].faild) {
        ExEvent.GameEvents.QuestionComplete.CallAsync(actionList.items[num]);
        //RunnerGamePlay.QuestComplited(actionList.items[num]);
      }

    }

    bool _subsctibeRunQuest;
    /// <summary>
    /// Подписываемся на необходимые события
    /// </summary>
    void SubscribeNeedsEvent(Quest needQuest) {

      if (!_subsctibeRunQuest && System.Array.IndexOf(new Quest[] {Quest.runDistance, Quest.runNoWeapon, Quest.runNoDamage, Quest.runDino,
                Quest.runSpider, Quest.runNoBarrier, Quest.noUseDoubleJump, Quest.runNoKillSpider}, needQuest) >= 0) {
        _subsctibeRunQuest = true;
        RunnerController.changeDistanceQuest += AddRun;
      }
    }

    /// <summary>
    /// Отписываемся
    /// </summary>
    /// <param name="needQuest"></param>
    void UnSubscribeEvent(Quest? needQuest = null) {

      bool allUnsubscribe = needQuest == null ? true : false;

      if (allUnsubscribe || _subsctibeRunQuest) {
        RunnerController.changeDistanceQuest -= AddRun;
        _subsctibeRunQuest = false;
      }

    }

    void CheckFullQuest() {

      if (CheckList()) {
        saveStatusSomplList(GameManager.activeLevel, true);

        RunnerController run = RunnerController.Instance;
        if (run != null) {
          run.keysInRace++;
          run.keysInRaceCalc++;
        }

      }
    }

    public static void byeElem(int num) {
      Instance.ByeElem(num);
    }

    public void ByeElem(int num) {
      actionList.items[num].value = actionList.items[num].needvalue;
      CheckQuestions(num);
    }

    void generateSFX(Question quest, Vector3 pos) {

      if (GameManager.activeLevel != GameManager.level) return;

      RunSpawner.questionSfx(pos);
      ExEvent.GameEvents.QuestionComplete.CallAsync(quest);
      //RunnerGamePlay.QuestComplited(quest);
    }

    /// <summary>
    /// Использовать Х раз буст
    /// Использовать Х раз буст ускоренного забега
    /// Использовать Х раз буст скейта
    /// Использовать Х раз буст бочки
    /// Использовать Х раз буст колесо мельницы
    /// Использовать Х раз буст корабля
    /// </summary>
    /// <param name="type"></param>
    public static void AddUseBoost(BoostType type) {
      Instance.addUseBoost(type);
    }
    public void addUseBoost(BoostType type) {
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;

      for (int i = 0; i < actionList.items.Length; i++) {

        if (actionList.items[i].type == Quest.useBoost
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
        }

        Quest thisType = Quest.useBoost;

        switch (type) {
          case BoostType.speed:
            thisType = Quest.useBoostSpeed;
            break;
          case BoostType.skate:
            thisType = Quest.useBoostSkate;
            break;
          case BoostType.barrel:
            thisType = Quest.useBoostBarrel;
            break;
          case BoostType.millWheel:
            thisType = Quest.useBoostMillwell;
            break;
          case BoostType.ship:
            thisType = Quest.useBoostShip;
            break;
        }

        if (System.Array.IndexOf(new Quest[] { Quest.useBoostSpeed, Quest.useBoostSkate, Quest.useBoostBarrel, Quest.useBoostMillwell, Quest.useBoostShip }, actionList.items[i].type) >= 0
            && actionList.items[i].type == thisType
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
        }
      }
    }
    /// <summary>
    /// Использовать X раз оружие общее
    /// Использовать Х раз копкан
    /// Использовать Х раз сабли
    /// Использовать Х раз пистолет
    /// Использовать Х раз бомбы
    /// Использовать Х раз молотов
    /// Использовать Х раз картечь
    /// Использовать Х видов оружия в 1 забеге
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pos"></param>
    public static void AddUseWeapon(WeaponTypes type, Vector3 pos) {
      Instance.addUseWeapon(type, pos);
    }
    public void addUseWeapon(WeaponTypes type, Vector3 pos) {
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;
      if (RunnerController.Instance.runnerPhase == RunnerPhase.tutorial) return;
      for (int i = 0; i < actionList.items.Length; i++) {
        if (actionList.items[i].type == Quest.useWeapon
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        Quest thisType = Quest.useBoost;

        switch (type) {
          case WeaponTypes.trap:
            thisType = Quest.useWeaponTrap;
            break;
          case WeaponTypes.sabel:
            thisType = Quest.useWeaponSabel;
            break;
          case WeaponTypes.gun:
            thisType = Quest.useWeaponPistol;
            break;
          case WeaponTypes.bomb:
            thisType = Quest.useWeaponBomb;
            break;
          case WeaponTypes.molotov:
            thisType = Quest.useWeaponMolotov;
            break;
          case WeaponTypes.ship:
            thisType = Quest.useWeaponBuckshot;
            break;
        }

        if (System.Array.IndexOf(new Quest[] { Quest.useWeaponTrap, Quest.useWeaponSabel, Quest.useWeaponPistol, Quest.useWeaponBomb, Quest.useWeaponMolotov, Quest.useWeaponBuckshot }, actionList.items[i].type) >= 0
            && actionList.items[i].type == thisType
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        if (!weaponList.Exists(x => x == type)) {
          weaponList.Add(type);

          // Проверка на типы используемого оружия
          if (actionList.items[i].type == Quest.useWeaponType
              && actionList.items[i].needvalue > actionList.items[i].value
              ) {
            actionList.items[i].value += 1;
            CheckQuestions(i);
            Instance.generateSFX(Instance.actionList.items[i], pos);
          }
        }

        if (actionList.items[i].type == Quest.use2weapon
                && actionList.items[i].needvalue > actionList.items[i].value
                && weaponList.Count >= 2
                ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        if (actionList.items[i].type == Quest.use3weapon
                && actionList.items[i].needvalue > actionList.items[i].value
                && weaponList.Count >= 3
                ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }
      }
    }
    /// <summary>
    /// Пробежать Х метров без использования оружия
    /// Пробежать Х не задев ни одного препядствия
    /// Пробежать Х метров без получения урона
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="noDamage"></param>
    /// <param name="noWeapon"></param>
    /// <param name="noBarrier"></param>
    /// <param name="noDoubleJump"></param>
    /// <param name="noKillSpider"></param>
    /// <param name="spiderPet"></param>
    /// <param name="dinoPet"></param>
    public static void addRun(float dist, bool noDamage, bool noWeapon, bool noBarrier, bool noDoubleJump, bool noKillSpider, bool spiderPet, bool dinoPet) {
      Instance.AddRun(dist, noDamage, noWeapon, noBarrier, noDoubleJump, noKillSpider, spiderPet, dinoPet);
    }
    float lastdist = 0;
    float deltaDist = 0;
    public void AddRun(float dist, bool noDamage, bool noWeapon, bool noBarrier, bool noDoubleJump, bool noKillSpider, bool spiderPet, bool dinoPet) {

      if (actionList.items == null || actionList.items.Length <= 0) return;

      deltaDist = dist - lastdist;
      lastdist = dist;


      for (int i = 0; i < actionList.items.Length; i++) {

        if (actionList.items[i].faild) continue;

        if (actionList.items[i].type == Quest.runDistance
            && actionList.items[i].needvalue > actionList.items[i].value
           ) {
          actionList.items[i].value += deltaDist;
          CheckQuestions(i);
        }

        if (actionList.items[i].type == Quest.runNoWeapon
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (noWeapon) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          } else if (!actionList.items[i].faild) {
            actionList.items[i].faild = true;
            CheckQuestions(i);
          }
        }


        if (actionList.items[i].type == Quest.runNoDamage
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (noDamage) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          } else if (!actionList.items[i].faild) {
            actionList.items[i].faild = true;
            CheckQuestions(i);
          }
        }

        if (actionList.items[i].type == Quest.runDino
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (dinoPet) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          }
        }


        if (actionList.items[i].type == Quest.runSpider
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (spiderPet) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          }
        }

        if (actionList.items[i].type == Quest.runNoBarrier
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (noBarrier) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          } else if (!actionList.items[i].faild) {
            actionList.items[i].faild = true;
            CheckQuestions(i);
          }
        }

        if (actionList.items[i].type == Quest.noUseDoubleJump
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (noDoubleJump) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          } else if (!actionList.items[i].faild) {
            actionList.items[i].faild = true;
            CheckQuestions(i);
          }
        }

        if (actionList.items[i].type == Quest.runNoKillSpider
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          if (noKillSpider) {
            actionList.items[i].value += deltaDist;
            CheckQuestions(i);
          } else if (!actionList.items[i].faild) {
            actionList.items[i].faild = true;
            CheckQuestions(i);
          }
        }

      }
    }
    /// <summary>
    /// Убить врага
    /// Убить ацтека
    /// Убить почти безголового зомби
    /// Убить ацтека с копьем
    /// Убить ацтека с бумерангом
    /// Убить гиганта
    /// Убить толстого зомби
    /// Убить врага копканом
    /// Убить врага саблей
    /// Убить врага пистолетом
    /// Убить врага бомбой
    /// Убить врага молотовым
    /// Убить врага картечью
    /// Убить врага над пропастью
    /// Убить врага в прыжке
    /// </summary>
    /// <param name="type"></param>
    /// <param name="weapon"></param>
    /// <param name="pos"></param>
    /// <param name="fly"></param>
    /// <param name="breaks"></param>
    public static void AddEnemyDead(EnemyTypes type, WeaponTypes weapon, Vector3 pos, bool fly = false, bool breaks = false) {
      Instance.addEnemyDead(type, weapon, pos, fly, breaks);
    }
    public void addEnemyDead(EnemyTypes type, WeaponTypes weapon, Vector3 pos, bool fly = false, bool breaks = false) {
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;
      if (RunnerController.Instance.runnerPhase == RunnerPhase.tutorial) return;
      for (int i = 0; i < actionList.items.Length; i++) {

        if (actionList.items[i].type == Quest.killEnemyBreack
            && actionList.items[i].needvalue > actionList.items[i].value
            && breaks
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        if (actionList.items[i].type == Quest.killEnemyJump
            && actionList.items[i].needvalue > actionList.items[i].value
            && fly
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        if (actionList.items[i].type == Quest.killEnemy
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }

        Quest thisType = Quest.useBoost;

        switch (type) {
          case EnemyTypes.aztec:
            thisType = Quest.killEnemyActec;
            break;
          case EnemyTypes.headlessZombie:
            thisType = Quest.killEnemyHeadlessZombie;
            break;
          case EnemyTypes.aztecSpear:
            thisType = Quest.killEnemyAztecSpear;
            break;
          case EnemyTypes.warriorBoomerang:
            thisType = Quest.killEnemyWarriorBoomerang;
            break;
          case EnemyTypes.fatZombie:
            thisType = Quest.killEnemyFatZombie;
            break;
          case EnemyTypes.warriorGiant:
            thisType = Quest.killEnemyWarriorGiant;
            break;
        }

        if (System.Array.IndexOf(new Quest[] { Quest.killEnemyActec, Quest.killEnemyHeadlessZombie, Quest.killEnemyAztecSpear, Quest.killEnemyWarriorBoomerang, Quest.killEnemyFatZombie, Quest.killEnemyWarriorGiant }, actionList.items[i].type) >= 0
            && actionList.items[i].type == thisType
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }


        switch (weapon) {
          case WeaponTypes.trap:
            thisType = Quest.killEnemyTrap;
            break;
          case WeaponTypes.sabel:
            thisType = Quest.killEnemySabel;
            break;
          case WeaponTypes.gun:
            thisType = Quest.killEnemyPistol;
            break;
          case WeaponTypes.bomb:
            thisType = Quest.killEnemyBomb;
            break;
          case WeaponTypes.molotov:
            thisType = Quest.killEnemyMolotov;
            break;
          case WeaponTypes.ship:
            thisType = Quest.killEnemyBuckshot;
            break;
        }

        if (System.Array.IndexOf(new Quest[] { Quest.killEnemyTrap, Quest.killEnemySabel, Quest.killEnemyPistol, Quest.killEnemyBomb, Quest.killEnemyMolotov, Quest.killEnemyBuckshot }, actionList.items[i].type) >= 0
            && actionList.items[i].type == thisType
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }
      }
    }
    /// <summary>
    /// Квест фейсбука
    /// </summary>
    public static void fbContact() {
      try {
        Instance.FbContact();
      } catch (System.Exception e) {
        Debug.Log(e.Message);
      }
    }
    public void FbContact() {
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;
      for (int i = 0; i < actionList.items.Length; i++) {
        if (actionList.items[i].type == Quest.contactFb
            && actionList.items[i].needvalue > actionList.items[i].value) {
          actionList.items[i].value += 1;
          CheckQuestions(i);
        }
      }
    }
    /// <summary>
    /// Пригласить друга
    /// </summary>
    /// <param name="count"></param>
    public static void fbInvite(int count) {
      Instance.FbInvite(count);
    }
    public void FbInvite(int count) {
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;
      for (int i = 0; i < actionList.items.Length; i++) {
        if (actionList.items[i].type == Quest.invateFriends
            && actionList.items[i].needvalue > actionList.items[i].value
            ) {
          actionList.items[i].value += count;
          CheckQuestions(i);
        }
      }
    }
    /// <summary>
    /// Обогнать
    /// </summary>
    /// <param name="pos"></param>
    public static void fbOvertakeFriend(Vector3 pos) {
      if (Instance == null) return;
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;

      for (int i = 0; i < Instance.actionList.items.Length; i++) {
        if (Instance.actionList.items[i].type == Quest.overtakeFriend
            && Instance.actionList.items[i].needvalue > Instance.actionList.items[i].value
            ) {
          Instance.actionList.items[i].value += 1;
          Instance.CheckQuestions(i);
          Instance.generateSFX(Instance.actionList.items[i], pos);
        }
      }
    }
    /// <summary>
    /// Отметка выполнения квеста
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="count"></param>
    /// <param name="sfxPosition"></param>
    public static void ConfirmQuestion(Quest quest, int count, Vector3 sfxPosition) {
      Instance.OnConfirmQuestion(quest, count, sfxPosition);
    }
    public static void ConfirmQuestion(Quest quest, int count) {
      Instance.OnConfirmQuestion(quest, count, Vector3.zero);
    }
    public void OnConfirmQuestion(Quest quest, int count, Vector3 sfxPosition) {
      if (Instance == null) return;
      if (Instance.actionList.items == null || Instance.actionList.items.Length <= 0) return;
      if (GameManager.activeLevelData.gameMode == GameMode.survival || RunnerController.Instance.runnerPhase == RunnerPhase.tutorial) return;

      for (int i = 0; i < Instance.actionList.items.Length; i++) {
        if (Instance.actionList.items[i].type == quest
        && Instance.actionList.items[i].needvalue > Instance.actionList.items[i].value
          ) {
          Instance.actionList.items[i].value += count;
          Instance.CheckQuestions(i);
          if (sfxPosition != Vector3.zero)
            Instance.generateSFX(Instance.actionList.items[i], sfxPosition);
        }
      }
    }

    public static void UpdateFromFbNow() {
      if (Instance == null) return;
      Instance.UpdateFromFb();
      Instance.CheckKeysTrans();
    }

    void UpdateFromFb() {

      int allGates = PlayerPrefs.GetInt("openGate", 0);

      int allComplitedQuest = 0;
      int allComplitedQuestBefore = 0;

      for (int i = 0; i < 100; i++)
        if (PlayerPrefs.GetInt("quastion" + "List" + i) == 1)
          allComplitedQuestBefore++;

      allComplitedQuest = allComplitedQuestBefore;

      if (allGates == 3 && allComplitedQuest < 30)
        allComplitedQuest = 30;
      else if (allGates == 2 && allComplitedQuest < 15)
        allComplitedQuest = 15;
      else if (allGates == 1 && allComplitedQuest < 5)
        allComplitedQuest = 5;

      if (allComplitedQuest > allComplitedQuestBefore) {
        for (int i = 0; i < allComplitedQuest; i++) {
          PlayerPrefs.SetInt("quastion" + "List" + i, 1);
        }
        PlayerPrefs.SetInt("keys", 0);
        InitQuestionsGroup();
      }

    }

    /// <summary>
    /// После изменения числа ключей с 50 до 25 возникла необходимость провести проверку на ситуацию
    /// при которой оставшееся число доступных ключей при 50 не мешали проходжению при 25
    /// </summary>
    void CheckKeysTrans() {

      //todo проанализировать и исправить
      return;

      if (RunnerController.Instance == null && MapController.Instance == null) return;
      int allComplitedQuest = -1;

      for (int i = 0; i < 100; i++)
        if (PlayerPrefs.GetInt("quastion" + "List" + i) == 1)
          allComplitedQuest = i;


      int allKeys = UserManager.Instance.keys;
      int allGates = PlayerPrefs.GetInt("openGate", 0);
      int newQuestionList = Questions.QuestionManager.Instance.companyList.Length;

      if (allComplitedQuest == -1 || Questions.QuestionManager.Instance.companyList.Length == 0) return;

      int allNeedKeys = 0;

      if (RunnerController.Instance != null) {
        for (int i = 0; i < GameManager.Instance.mapRun.Count; i++)
          if (allGates < i + 1)
            allNeedKeys += (int)GameManager.Instance.mapRun[i].keys;
      } else if (MapController.Instance != null) {

        for (int i = 0; i < MapController.Instance.gate.Length; i++)
          if (allGates < i + 1)
            allNeedKeys += (int)MapController.Instance.gate[i].needKeys;
      }

      int needKeys = allNeedKeys - allKeys;
      int allReadyQuest = (newQuestionList - (allComplitedQuest + 1));

      if (allReadyQuest < needKeys) {
        ClearAllQuestionComplited();

        for (int i = 0; i < newQuestionList - needKeys; i++) {
          PlayerPrefs.SetInt("quastion" + "List" + i, 1);
        }

        InitQuestionsGroup();
      }

    }

    void TestHaveQuest() {

      int keys = 35;

      for (int i = 0; i < keys; i++) {
        PlayerPrefs.SetInt("quastion" + "List" + i, 1);
      }
      if (keys >= 50) {
        PlayerPrefs.SetInt("openGate", 3);
        keys -= 50;
      } /*else if (keys >= 25) {
            PlayerPrefs.SetInt("openGate", 2);
            keys -= 25;
        }*/ else if (keys >= 10) {
        PlayerPrefs.SetInt("openGate", 1);
        keys -= 10;
      }

      PlayerPrefs.SetInt("keys", keys);
    }

  }

  [System.Serializable]
  public struct DescriptionLib {
    public string name;
    public Quest type;
  }

  // Структура одного квеста
  [System.Serializable]
  public struct Question {
    public Quest type;              // Тип квеста
    public float needvalue;             // Дребуемое значение
    public float value;                 // Текущее значение
    public string descr;                // Описание Квеста
    public int price;
    public bool cumulative;             // Накапительное
    public bool faild;

    public string titleTranslate {
      get { return LanguageManager.GetTranslate("quest_" + type.ToString()) + (!cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : ""); }
    }

    public string countGui {
      get { return value + "/" + needvalue; }
    }
  }

  // Структура элемента списка
  [System.Serializable]
  public struct OneListQuestions {
    public bool listComplited;          // Флаг об окончании квеста
    public Question[] items;            // Необходимые квесты
  }


  // Структура элемента списка
  [System.Serializable]
  public struct LevelList {
    public bool isComplete;          // Флаг об окончании квеста
    public Question[] items;            // Необходимые квесты
  }

}