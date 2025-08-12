using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WordIsland : MonoBehaviour {

	public GameObject letterPrefab;
	private List<GameLetter> prefabListLetter = new List<GameLetter>();
	public List<GameLetter> listLetter = new List<GameLetter>();

	public float sizeLetter;
	public float distanceXLetter;

	public GameCompany.Word word;

	public bool isOpen = false;

	public float diffScale { get; set; }

	public float summLenght {
		get { return diffScale * 2 + sizeLetter; }
	}

	public void SetWord(GameCompany.Word word) {

		HideExists();

    word.starCount = 0;


    this.word = word;

		GameCompany.Save.Level _saveLevel = PlayerManager.Instance.company.GetSaveLevel();

		// Определяем смещение начала слова в локальных координатах
		float allLenght = word.word.Length * sizeLetter + (word.word.Length - 1) * distanceXLetter;
		diffScale = (allLenght / 2 - sizeLetter / 2);

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
		for (int i = 0; i < word.word.Length; i++) {
			GameLetter let = GetInstance();
			let.gameObject.SetActive(true);
			let.transform.localPosition = new Vector3(-diffScale + i * sizeLetter + (i <= 0 ? 0 : i * distanceXLetter), 0, 0);
			let.SetData(i, word.word[i].ToString());
			let.OnClick = ClickLetter;
			let.SetStatus(isOpen);

			let.isCoin = !PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial && (word.coinsLetters != null && word.coinsLetters.Contains(i) && !isOpen && !hintLetters.Contains(i));

			listLetter.Add(let);
		}
		
		SetHints(hintLetters, true);
		
		if (word.starExists && !PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial) {
			CreateStar();
			showStars.ForEach(x=>x.gameObject.SetActive(true));
			SetStarReady(true);
		}

	}

	public void VisualWord(bool isFirst) {
		StartCoroutine(ShowLetters(isFirst));
		//listLetter.ForEach(x=>x.Animated(isFirst));
	}

	public void HideAll() {
		StopAllCoroutines();
		HideExists();
	}

	public bool ExistsHint() {
		return listLetter.Exists(x => x.isHint);
	}

	public bool ExistsOpen() {
		return listLetter.Exists(x => x.isOpen);
	}

	IEnumerator ShowLetters(bool isFirst) {
		for (int i = 0; i < listLetter.Count; i++) {
			if (listLetter[i].isOpen || listLetter[i].isHint || isFirst) {
				listLetter[i].Animated(isFirst);
				yield return new WaitForSeconds(0.1f);
			}
		}
		if (word.starExists) {
			if (isFirst)
				showStars.ForEach(x => x.Visual());
			else
				SetStarReady(false, false);
		}
	}
	
	public void SetStarReady(bool isFirst = false, bool effects = true) {
		//parentStar.transform.localPosition = new Vector3(diffScale + sizeLetter / 2, sizeLetter / 2, 0);

		//showStars.ForEach(x=>x.SetOpen(isOpen, isFirst, effects));

		//if(!isFirst)
		//	showStars.ForEach(x => x.Visual());
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

	/// <summary>
	/// Обработка события клика по любому из букв
	/// </summary>
	/// <param name="numLetter">Номер буквы по счету</param>
	private void ClickLetter(int numLetter) {
		
		if ((!listLetter[numLetter].isHint && !listLetter[numLetter].isOpen) && PlayerManager.Instance.isHintAnyLetter) {
			listLetter[numLetter].SetHint(false);
			ExEvent.GameEvents.OnHintAnyLetterCompleted.Call(true, word.word, numLetter);
		}
		else if(!PlayerManager.Instance.isHintAnyLetter) {
			ShowTranslate();
		}
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

	void HideExists() {
		prefabListLetter.ForEach(x => x.gameObject.SetActive(false));
		listLetter.Clear();
	}

	public void Open() {
		if(gameObject.activeInHierarchy)
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

	public bool CheckOpenFirst() {
		return listLetter[0].isOpen;
	}

	public bool CheckHintFirst() {
		return listLetter[0].isHint;
	}

	public bool CheckFullHint() {
		for (int i = 0; i < listLetter.Count; i++) {
			if (!listLetter[i].isHint)
				return false;
		}
		return true;
	}

	public void ShowTranslate() {
		if (!isOpen) return;
		ExEvent.GameEvents.ShowWordTranslate.Call(word);
	}


	public Transform parentStar;
	public WordStar starPrefab;
	private List<WordStar> starInstance = new List<WordStar>();
	private List<WordStar> showStars = new List<WordStar>();

	public void CreateStar() {
		HideAllStar();
		//for (int i = 0; i < word.starCount; i++) {
		//	WordStar star = GetInstanceStar();
		//	star.gameObject.SetActive(true);
		//	star.transform.localPosition = new Vector3(i* -0.45f, 0,0);
		//	star.transform.localScale = Vector3.one;
		//	showStars.Add(star);
		//}
	}
	public void StarShow(bool isShow) {
		showStars.ForEach(x => x.gameObject.SetActive(isShow));
	}

	private void HideAllStar() {
		showStars.ForEach(x=>x.gameObject.SetActive(false));
		showStars.Clear();
	}

	private WordStar GetInstanceStar() {

		WordStar star = starInstance.Find(x => !x.isActiveAndEnabled);
		if (star == null) {
			GameObject inst = Instantiate(starPrefab.gameObject);
			inst.transform.SetParent(parentStar);
			star = inst.GetComponent<WordStar>();
			starInstance.Add(star);
		}
		return star;
	}

}
