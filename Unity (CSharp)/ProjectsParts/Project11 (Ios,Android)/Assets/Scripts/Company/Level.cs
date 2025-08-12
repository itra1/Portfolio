using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany {

	[System.Serializable]
	public class Level {
		public int id;
		public string title;
		public List<Word> words;    // Слова
		public List<string> letters;      // Буквы
		public int version;
    public List<Crossword.Letter> crosswordLetter = new List<Crossword.Letter>();
    public Vector2Int crosswordSize;
    public Vector2Int startPosition;


		public static Level CreateInstance() {
			Level lvl = new Level();
			lvl.words = new List<Word>();
			lvl.letters = new List<string>();
			return lvl;
		}


		public List<string> randomLetters {
			get {

				List<string> newList = new List<string>();
				List<string> tmpList = new List<string>(letters);

				while (tmpList.Count > 0) {
					int num = Random.Range(0, tmpList.Count);
					newList.Add(tmpList[num]);
					tmpList.RemoveAt(num);
				}
				return newList;

			}
		}

		public int GetCountSecretWords() {
			int totlWords = 0;
			for (int i = 0; i < words.Count; i++)
				if (!words[i].primary)
					totlWords++;
			return totlWords;
		}

		public int GetCountPrimaryWords() {
			int totlWords = 0;
			for (int i = 0; i < words.Count; i++)
				if (words[i].primary)
					totlWords++;
			return totlWords;
		}

		public List<Word> GetAllPrimaryWords() {
			List<Word> wordsPrimary = new List<Word>();
			for (int i = 0; i < words.Count; i++)
				if (words[i].primary)
					wordsPrimary.Add(words[i]);
			return wordsPrimary;
		}

		public List<Word> GetAllPrimaryNoStarWords() {
			List<Word> wordsPrimary = new List<Word>();
			for (int i = 0; i < words.Count; i++)
				if (words[i].primary && !words[i].starExists)
					wordsPrimary.Add(words[i]);
			return wordsPrimary;
		}

		public int GetCountStar() {
			int totalStar = 0;
			for (int i = 0; i < words.Count; i++)
				if (words[i].starExists)
					totalStar+= words[i].starCount;
			return totalStar;
		}

		public List<Word> GetAllStarWords() {
			List<Word> wordsPrimary = new List<Word>();
			for (int i = 0; i < words.Count; i++)
				if (words[i].starExists)
					wordsPrimary.Add(words[i]);
			return wordsPrimary;
		}

		// Проверка возможности добавить звезду к слову
		public int CountAddStarToWord(Word word, Save.Level saveLevel) {

			if (GetCountStar() == 3) return 0;

			List<Word> primaryNoStart = GetAllPrimaryNoStarWords();

			if (primaryNoStart.Count == 1) return 3- GetCountStar();

			if (primaryNoStart.Count > 1) {
				List<Word> noOpen = primaryNoStart.FindAll(x => !saveLevel.words.Exists(w => w.word == x.word && w.isOpen));

				if(noOpen.Count == 1 && noOpen[0].word == word.word)
					return 3 - GetCountStar();
			}
			return 0;

		}

	}

}
