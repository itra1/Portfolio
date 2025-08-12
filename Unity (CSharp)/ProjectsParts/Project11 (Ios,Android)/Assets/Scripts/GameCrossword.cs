using System.Collections;
using System.Collections.Generic;
using GameCompany;
using UnityEngine;

public class GameCrossword: Singleton<GameCrossword> {

  public Transform parentWords;
  public GameWord wordPrefab;
  [HideInInspector]
  public List<GameWord> wordInstanceList = new List<GameWord>();
  [HideInInspector]
  public List<GameWord> wordList = new List<GameWord>();
  [HideInInspector]
  public List<GameCompany.Word> wordsList = new List<GameCompany.Word>();

  public List<GameLetter> letterList = new List<GameLetter>();
  Level gameLevel;
  public float horizontal;
  public float verticel;

  private void OnDrawGizmos() {
    Gizmos.DrawWireCube(transform.position, new Vector3(horizontal, verticel, 0));
  }

  public void InitData() {

    HideExists();

    gameLevel = PlayerManager.Instance.company.GetActualLevel();
    GameCompany.Save.Level saveGameLevel = PlayerManager.Instance.company.GetSaveLevel();

    wordsList = gameLevel.words.FindAll(x=>x.primary);

    DrawWordList();

  }

  private void DrawWordList() {

    letterList.ForEach(x => Destroy(x));
    letterList.Clear();

    Vector2 startPosition = new Vector2((float)gameLevel.crosswordSize.x / 2f - .5f, (float)gameLevel.crosswordSize.y / 2f - .5f);

    for (int i = 0; i < wordsList.Count; i++) {

      List<GameWord> useWords = new List<GameWord>();

      GameWord word = GetInstance();
      word.gameObject.SetActive(true);
      word.SetWord(this, wordsList[i]);
      word.transform.localScale = new Vector3(1, 1, 1);
      useWords.Add(word);
      wordList.Add(word);

      word.transform.localPosition = new Vector3(-startPosition.x + wordsList[i].crosswordWord.letterList[0].position.x + Mathf.Abs(gameLevel.startPosition.x), startPosition.y - wordsList[i].crosswordWord.letterList[0].position.y - Mathf.Abs(gameLevel.startPosition.y), 0);

    }
    if (gameLevel.crosswordSize.x > horizontal || gameLevel.crosswordSize.y > verticel) {

      float perct = horizontal / gameLevel.crosswordSize.x;
      float perct2 = verticel / gameLevel.crosswordSize.y;

      parentWords.localScale = new Vector3(Mathf.Min(perct, perct2), Mathf.Min(perct, perct2));
    } else
      parentWords.localScale = new Vector3(1, 1);

  }

  private GameWord GetInstance() {
    GameWord wi = wordInstanceList.Find(x => !x.gameObject.activeInHierarchy);
    if (wi == null) {
      GameObject inst = Instantiate(wordPrefab.gameObject);
      inst.transform.SetParent(parentWords);
      wi = inst.GetComponent<GameWord>();
      wordInstanceList.Add(wi);
    }
    return wi;
  }

  public void ShowLetter() {
    if (showWordCor != null)
      StopCoroutine(showWordCor);
    showWordCor = StartCoroutine(ShowWord());
  }

  private Coroutine showWordCor;
  IEnumerator ShowWord() {
    //VisualWord(true);
    yield return ShowLetters(true);
    yield return ShowLetters(false);

    //for (int i = 0; i < wordList.Count; i++) {
    //  if (wordList[i].ExistsHint() || wordList[i].ExistsOpen()) {
    //    VisualWord(false);
    //    yield return new WaitForSeconds(wordList[i].word.word.Length * 0.1f);
    //  }
    //}

    ExEvent.GameEvents.OnLetterLoaded.Call();
  }

  public void VisualWord(bool isFirst) {
    StartCoroutine(ShowLetters(isFirst));
    //listLetter.ForEach(x=>x.Animated(isFirst));
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
  public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

    if (selectWord.select != SelectWord.yes) return;

    GameWord gw = wordInstanceList.Find(x => x.word.word == selectWord.word);
    if (gw == null) return;

    gw.isOpen = true;
    
    StartCoroutine(OneLetter(gw));
    
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintFirstWordReady))]
  public void OnHintFirstWord(ExEvent.GameEvents.OnHintFirstWordReady hint) {

    foreach (var elem in wordInstanceList) {
      if (!elem.isOpen && !elem.CheckFullHint()) {
        //elem.Open();
        elem.SetHints();
        ExEvent.GameEvents.OnHintFirstWordCompleted.Call(true, elem.word.word);
        return;
      }
    }
    ExEvent.GameEvents.OnHintFirstWordCompleted.Call(false);
  }

  IEnumerator ShowLetters(bool isFirst) {
    for (int i = 0; i < letterList.Count; i++) {
      if (letterList[i].isOpen || letterList[i].isHint || isFirst) {
        letterList[i].Animated(isFirst);
        yield return new WaitForSeconds(0.1f);
      }
    }
  }

  IEnumerator OneLetter(GameWord gameWord) {

    for (int i = 0; i < gameWord.listLetter.Count; i++) {
      gameWord.listLetter[i].SetStatus(true, true);
      yield return new WaitForSeconds(0.07f);
    }

    gameWord.ShowTranslate();
  }

  public void HideExists() {
    wordInstanceList.ForEach(x => x.gameObject.SetActive(false));
  }


}
