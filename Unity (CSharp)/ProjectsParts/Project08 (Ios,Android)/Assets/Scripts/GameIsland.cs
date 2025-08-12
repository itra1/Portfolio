using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using GameCompany;
using UnityEngine;

public class GameIsland : ExEvent.EventBehaviour {

	public Transform parentWords;
	public WordIsland wordPrefab;
	public List<WordIsland> wordIslandInstanceList = new List<WordIsland>();
	public List<WordIsland> wordIslandList = new List<WordIsland>();
	public List<GameCompany.Word> wordsList = new List<Word>();
	private WordIsland lastOpenWord;
	public SpriteRenderer island;

	public float horizontal;
	public float verticel;

	public IslandDecorsManager decorIsland;

	private void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position, new Vector3(horizontal, verticel, 0));
	}

	public void InitData() {

		if (!PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial) {
			var grph = GraphicManager.Instance.link.GetGraphic(PlayerManager.Instance.company.actualLocationNum / 3 + 1);
			island.sprite = grph.gameBack;
		}

		HideExists();

		Level gameLevel = PlayerManager.Instance.company.GetActualLevel();
		GameCompany.Save.Level saveGameLevel = PlayerManager.Instance.company.GetSaveLevel();

		wordsList = gameLevel.words;

		// Если слов меньше 3х с последнему добавляем недостоющее число слов
		if (gameLevel.GetCountPrimaryWords() < 3 && gameLevel.GetCountStar() < 3) {
			var checkList = gameLevel.GetAllPrimaryWords();
			checkList[checkList.Count - 1].starCount += 3 - gameLevel.GetCountStar();
		}

		CalcScaling();

		List<List<GameCompany.Word>> wordsLine = new List<List<Word>>();
		wordsLine.Add(new List<GameCompany.Word>());

		int line = 0;
		int letterInLine = 0;

		foreach (var elem in wordsList) {
			if (!elem.primary) continue;

			if (!elem.starExists)
				elem.starCount = gameLevel.CountAddStarToWord(elem, saveGameLevel);

			if (letterInLine + elem.word.Length <= _maxCountInRow) {
				letterInLine += elem.word.Length;
			} else {
				line++;
				letterInLine = elem.word.Length;
				wordsLine.Add(new List<GameCompany.Word>());
			}
			wordsLine[line].Add(elem);
		}

		float startPositionY = ((wordsLine.Count * wordPrefab.sizeLetter * _sizeKoeff) +
													 ((wordsLine.Count - 1) * wordPrefab.distanceXLetter * 2.5f * _sizeKoeff)) / 2 - (wordPrefab.sizeLetter / 2 * _sizeKoeff);


		wordIslandList.Clear();

		parentWords.localScale = Vector3.one;

		float maxRightPosition = 0;

		// Отрисовка в линию
		for (int i = 0; i < wordsLine.Count; i++) {

			float summaryLenght = 0;
			List<WordIsland> useWords = new List<WordIsland>();

			for (int x = 0; x < wordsLine[i].Count; x++) {

				WordIsland word = GetInstance();
				word.gameObject.SetActive(true);
				word.SetWord(wordsLine[i][x]);
				word.transform.localScale = new Vector3(_sizeKoeff, _sizeKoeff, _sizeKoeff);
				useWords.Add(word);
				wordIslandList.Add(word);

				Vector3 targetPos = Vector3.zero;
				targetPos.y = startPositionY - i * wordPrefab.sizeLetter * _sizeKoeff - (i <= 0 ? 0 : i * wordPrefab.distanceXLetter * _sizeKoeff * 2.5f);
				targetPos.x = 0;
				word.transform.localPosition = targetPos;

				summaryLenght += word.summLenght * _sizeKoeff;
				if (x > 0) summaryLenght += wordPrefab.distanceXLetter * 5 * _sizeKoeff;
			}

			float lineStartX = -summaryLenght / 2;

			if (lineStartX < maxRightPosition)
				maxRightPosition = lineStartX;

			for (int x = 0; x < useWords.Count; x++) {
				if (useWords.Count <= 1) continue;

				Vector3 targetPos = useWords[x].transform.localPosition;
				targetPos.x = lineStartX + useWords[x].summLenght / 2 * _sizeKoeff;
				useWords[x].transform.localPosition = targetPos;
				lineStartX += useWords[x].summLenght * _sizeKoeff + wordPrefab.distanceXLetter * 5 * _sizeKoeff;
			}

		}

		if (startPositionY + _sizeKoeff / 2 > verticel / 2) {
			float scaleCoeff = (verticel / 2) / (startPositionY + _sizeKoeff / 2);
			parentWords.localScale *= scaleCoeff;
			maxRightPosition *= scaleCoeff;
		}
		if (Mathf.Abs(maxRightPosition) > horizontal / 2) {
			float scaleCoeff = (horizontal / 2) / Mathf.Abs(maxRightPosition);
			parentWords.localScale *= scaleCoeff;
		}

		CheckNoOpen();
		decorIsland.PositionsDecor();
	}

	public void ShowLetter() {
		if (showWordCor != null)
			StopCoroutine(showWordCor);
		showWordCor = StartCoroutine(ShowWord());
	}

	private Coroutine showWordCor;
	IEnumerator ShowWord() {
		for (int i = 0; i < wordIslandList.Count; i++) {
			wordIslandList[i].VisualWord(true);
			yield return new WaitForSeconds(wordIslandList[i].word.word.Length * 0.1f);
		}

		for (int i = 0; i < wordIslandList.Count; i++) {
			if (wordIslandList[i].ExistsHint() || wordIslandList[i].ExistsOpen()) {
				wordIslandList[i].VisualWord(false);
				yield return new WaitForSeconds(wordIslandList[i].word.word.Length * 0.1f);
			}
		}

		ExEvent.GameEvents.OnLetterLoaded.Call();
	}

	private WordIsland GetInstance() {
		WordIsland wi = wordIslandInstanceList.Find(x => !x.gameObject.activeInHierarchy);
		if (wi == null) {
			GameObject inst = Instantiate(wordPrefab.gameObject);
			inst.transform.SetParent(parentWords);
			wi = inst.GetComponent<WordIsland>();
			wordIslandInstanceList.Add(wi);
		}
		return wi;
	}

	public void HideExists() {
		wordIslandInstanceList.ForEach(x => x.gameObject.SetActive(false));
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
	public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

		if (selectWord.select != SelectWord.yes) return;

		wordIslandInstanceList.ForEach((elem) => {
			if (elem.word.word == selectWord.word) {
				elem.Open();
				lastOpenWord = elem;
			}
		});

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBattleChangePhase))]
	public void OnLevelCompleted(ExEvent.GameEvents.OnBattleChangePhase selectWord) {
		/*
		Level gameLevel = PlayerManager.Instance.GetActualLevel();

		int starCount = gameLevel.GetCountStar();

		if (starCount < 3) {
			WordIsland useWord = lastOpenWord;
			if (lastOpenWord == null) {
				useWord = wordIslandList[wordIslandList.Count - 1];
			}
			useWord.word.starCount++;
			useWord.CreateStar();
			useWord.SetStarReady();
			useWord.StarShow(true);
		}
		*/
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	public void OnChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase phase) {
		if (phase.last == GamePhase.game && phase.next != GamePhase.game) {
			wordIslandList.ForEach(x => x.HideAll());
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSave))]
	public void OnWordSave(ExEvent.GameEvents.OnWordSave selectWord) {
		CheckNoOpen();
	}

	private void CheckNoOpen() {

		if (PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial) return;

		Level gameLevel = PlayerManager.Instance.company.GetActualLevel();
		GameCompany.Save.Level saveGameLevel = PlayerManager.Instance.company.GetSaveLevel();

		for (int i = 0; i < wordIslandList.Count; i++) {

			if (!wordIslandList[i].word.starExists) {
				int newStarCount = gameLevel.CountAddStarToWord(wordIslandList[i].word, saveGameLevel);
				if (newStarCount > 0) {
					wordIslandList[i].word.starCount = newStarCount;
					wordIslandList[i].CreateStar();
					wordIslandList[i].SetStarReady();
					wordIslandList[i].StarShow(true);
				}
			}

		}


	}

	private float _sizeKoeff;
	private int _maxCountInRow;
	private int _rowCount;
	private void CalcScaling() {

		int alphaCount = 0;

		for (int i = 0; i < wordsList.Count; i++) {
			if (wordsList[i].primary)
				alphaCount += wordsList[i].word.Length;
		}

		if (alphaCount <= 15) {
			_sizeKoeff = 1;
			_maxCountInRow = 5;
			_rowCount = 3;
			return;
		}

		if (alphaCount <= 19) {
			_sizeKoeff = 0.87f;
			_maxCountInRow = 7;
			_rowCount = 4;
			return;
		}

		if (alphaCount <= 25) {
			_sizeKoeff = 0.85f;
			_maxCountInRow = 8;
			_rowCount = 4;
			return;
		}

		if (alphaCount <= 30) {
			_sizeKoeff = 0.7f;
			_maxCountInRow = 8;
			_rowCount = 4;
			return;
		}

		if (alphaCount <= 40) {
			_sizeKoeff = 0.65f;
			_maxCountInRow = 10;
			_rowCount = 5;
			return;
		}

		if (alphaCount <= 55) {
			_sizeKoeff = 0.6f;
			_maxCountInRow = 10;
			_rowCount = 6;
			return;
		}
		_sizeKoeff = 0.6f;
		_maxCountInRow = 10;
		_rowCount = 6;

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintFirstLetterReady))]
	public void OnHintFirstLetter(ExEvent.GameEvents.OnHintFirstLetterReady hint) {

		foreach (var elem in wordIslandInstanceList) {
			if (!elem.isOpen && !elem.CheckFullHint()) {
				if (!elem.CheckOpenFirst() && !elem.CheckHintFirst()) {
					elem.SetHints(new List<int> { 0 });
					ExEvent.GameEvents.OnHintFirstLetterCompleted.Call(true, elem.word.word, 0);
					return;
				}
			}
		}
		ExEvent.GameEvents.OnHintFirstLetterCompleted.Call(false);

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintFirstWordReady))]
	public void OnHintFirstWord(ExEvent.GameEvents.OnHintFirstWordReady hint) {

		foreach (var elem in wordIslandInstanceList) {
			if (!elem.isOpen && !elem.CheckFullHint()) {
				//elem.Open();
				elem.SetHints();
				ExEvent.GameEvents.OnHintFirstWordCompleted.Call(true, elem.word.word);
				return;
			}
		}
		ExEvent.GameEvents.OnHintFirstWordCompleted.Call(false);
	}

}
