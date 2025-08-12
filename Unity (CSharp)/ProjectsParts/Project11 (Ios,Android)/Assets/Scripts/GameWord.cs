using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWord: MonoBehaviour {

  public GameObject letterPrefab;
  private List<GameLetter> prefabListLetter = new List<GameLetter>();
  public List<GameLetter> listLetter = new List<GameLetter>();

  public float sizeLetter;
  public float distanceXLetter;

  public GameCompany.Word word;

  public bool isOpen = false;

  public void SetWord(GameCrossword gameCrossword, GameCompany.Word word) {

    HideExists();

    this.word = word;

    GameCompany.Save.Level _saveLevel = PlayerManager.Instance.company.GetSaveLevel(PlayerManager.Instance.company.actualCompany, PlayerManager.Instance.company.actualLevelNum);

    // Определяем открыто ли слово
    isOpen = false;
    List<int> hintLetters = new List<int>();
    if (_saveLevel != null) {
      for (int i = 0; i < _saveLevel.words.Count; i++)
        if (_saveLevel.words[i].word == word.word) {
          isOpen = _saveLevel.words[i].isOpen;
          hintLetters = _saveLevel.words[i].hintLetters;
        }
    }

    // Генерируем буквы
    for (int i = 0; i < word.crosswordWord.letterList.Count; i++) {

      GameLetter gl = gameCrossword.letterList.Find(l => l.crosswordLetter != null && l.crosswordLetter.position == word.crosswordWord.letterList[i].position);

      if (gl != null) {

        if (!gl.isOpen && isOpen)
          gl.SetStatus(isOpen);

        listLetter.Add(gl);
        continue;
      }

      GameLetter let = GetInstance();
      let.gameObject.SetActive(true);
      let.crosswordLetter = word.crosswordWord.letterList[i];

      if (word.crosswordWord.orientation == Crossword.Orientation.horizontal) {
        let.transform.localPosition = new Vector3(i * sizeLetter + (i <= 0 ? 0 : i * distanceXLetter), 0, 0);
      } else {
        let.transform.localPosition = new Vector3(0, -i * sizeLetter - (i <= 0 ? 0 : i * distanceXLetter), 0);
      }

      let.isCoin = word.crosswordWord.letterList[i].isCoin;

      let.SetData(i, word.word[i].ToString());
      let.OnClick = ClickLetter;
      let.SetStatus(isOpen);
      gameCrossword.letterList.Add(let);

      //let.isCoin = !PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial && (word.coinsLetters != null && word.coinsLetters.Contains(i) && !isOpen && !hintLetters.Contains(i));

      listLetter.Add(let);
    }

    SetHints(hintLetters, true);

  }

  void HideExists() {
    prefabListLetter.ForEach(x => x.gameObject.SetActive(false));
    listLetter.Clear();
  }

  private GameLetter GetInstance() {

    GameLetter li = prefabListLetter.Find(x => !x.gameObject.activeInHierarchy);

    if (li == null) {
      GameObject inst = Instantiate(letterPrefab);
      inst.transform.SetParent(transform);
      li = inst.GetComponent<GameLetter>();
      prefabListLetter.Add(li);
    }
    return li;
  }

  public void SetHints(List<int> hintLetters = null, bool isFirst = false) {
    if (hintLetters == null) {
      listLetter.ForEach(x => x.SetHint(isFirst));
      return;
    }

    for (int i = 0; i < listLetter.Count; i++)
      if (hintLetters.Contains(i))
        listLetter[i].SetHint(isFirst);
  }

  private void ClickLetter(int numLetter) {

    if ((!listLetter[numLetter].isHint && !listLetter[numLetter].isOpen) && PlayerManager.Instance.isHintAnyLetter) {
      listLetter[numLetter].SetHint(false);
      ExEvent.GameEvents.OnHintAnyLetterCompleted.Call(true, word.word, numLetter);
    } else if (!PlayerManager.Instance.isHintAnyLetter) {
      Crossword.Letter let = word.crosswordWord.letterList[numLetter];

      List<GameWord> gw = GameCrossword.Instance.wordList.FindAll(x => x.word.crosswordWord.letterList.Exists(l => l.position == let.position) && x.isOpen);

      if (gw.Count > 1 || gw.Count == 0) return;

      //if(GameCrossword.Instance.wordList.FindAll(x => x.word.crosswordWord.letterList.Exists(l => l.position == let.position) && x.isOpen).Count <= 1)
      //  ShowTranslate();
      ExEvent.GameEvents.ShowWordTranslate.Call(gw[0].word);
    }
  }

  public void ShowTranslate() {
    //if (!isOpen) return;
    ExEvent.GameEvents.ShowWordTranslate.Call(word);
  }


  public Transform parentStar;
  public WordStar starPrefab;
  private readonly List<WordStar> starInstance = new List<WordStar>();
  private List<WordStar> showStars = new List<WordStar>();

  public bool ExistsHint() {
    return listLetter.Exists(x => x.isHint);
  }

  public bool ExistsOpen() {
    return listLetter.Exists(x => x.isOpen);
  }

  public bool CheckFullHint() {
    for (int i = 0; i < listLetter.Count; i++) {
      if (!listLetter[i].isHint)
        return false;
    }
    return true;
  }

  public void SetStarReady(bool isFirst = false, bool effects = true) {
    //parentStar.transform.localPosition = new Vector3(diffScale + sizeLetter / 2, sizeLetter / 2, 0);

    //showStars.ForEach(x=>x.SetOpen(isOpen, isFirst, effects));

    //if(!isFirst)
    //	showStars.ForEach(x => x.Visual());
  }

  public void Open() {
    if (gameObject.activeInHierarchy)
      StartCoroutine(ShowCoro());
    isOpen = true;
  }

  IEnumerator ShowCoro() {

    for (int i = 0; i < listLetter.Count; i++) {
      listLetter[i].SetStatus(true, true);
      yield return new WaitForSeconds(0.07f);
    }

    if (word.starExists) {
      showStars.ForEach(x => x.SetOpen(true));
    }

    ShowTranslate();
  }

}
