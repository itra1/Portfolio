using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : Singleton<QuestionManager> {

  //[HideInInspector]
  public List<Question> readyQuestion = new List<Question>();
  public List<Question> questionList;
  
  private Queue<Question> rewardOrder = new Queue<Question>();

  private void Start() {
    Load();

    InitList();
  }

  private void Load() {
    questionList.ForEach(elem => {

      if(PlayerPrefs.HasKey("quest" + elem.id)) {
        QuestSave qs = JsonUtility.FromJson<QuestSave>(PlayerPrefs.GetString("quest" + elem.id));
        elem.value = qs.value;
        elem.isGet = qs.isGet;
      }
      
    });
  }

  public void InitList() {

    readyQuestion.Clear();

    for (int i = 0; i < 3; i++) {
      Question q = GetNextQuest();
      if (q != null)
        readyQuestion.Add(q);
    }
  }

  public void Save(Question elem) {

    QuestSave sq = new QuestSave {
      value = elem.value,
      isGet = elem.isGet
    };

    PlayerPrefs.SetString("quest" + elem.id, JsonUtility.ToJson(sq));
  }

  public void AddValueQuest(Type type,float value) {

    Question q = readyQuestion.Find(x => x.type == type);
    if (q == null) return;
    q.value += value;
    Save(q);
  }

  private Question GetNextQuest() {
    return questionList.Find(x => !x.isGet && !readyQuestion.Contains(x));
  }

  [System.Serializable]
  public class Question {
    public Type type;
    [UUID]
    public string id;
    public string title;
    public string description;
    public float count;
    //[HideInInspector]
    public float value;
    public bool isGet = false;

    public List<Reward> rewardList;

    public bool isComplete {
      get {
        return value >= count;
      }
    }

  }

  [System.Serializable]
  public class Reward {

    public Config.ResourceType type;
    public float count;
    
  }

  public void AddGetReward(Question quest) {
    rewardOrder.Enqueue(quest);
    ShowNewReward();
  }

  private bool rewatdIsShow = false;
  public void ShowNewReward() {

    if (rewatdIsShow || rewardOrder.Count <= 0) return;

    Question q = rewardOrder.Dequeue();

    rewatdIsShow = true;
    RewardQuestDialog dialog = UiController.GetUi<RewardQuestDialog>();
    dialog.Show();
    dialog.SetData(q);
  }

  public void RewardComplete() {
    rewatdIsShow = false;

    ShowNewReward();
  }

  [ContextMenu("Text complete quest")]
  public void DEBUG_TestRewardComplete() {
    AddGetReward(readyQuestion[0]);
    AddGetReward(readyQuestion[1]);
  }
  
  public class QuestSave {
    public float value = 0;
    public bool isGet = false;
  }

  /// <summary>
  /// Тип квеста
  /// </summary>
  public enum Type {
    none,               // Никакой
    getBullets,         // Собрать патрон
    byeChest,           // Покупка сундука
    getMegicine,        // Собрать аптечки
    killEnemy,          // Убить врагов
    killBoss,           // Убить босов
    completeLocation,   // Окончить уровень
    click,              // Клики
    getWeapon,          // Добавить оружие

    buyBullet           // Купить снаряды
  }

}
