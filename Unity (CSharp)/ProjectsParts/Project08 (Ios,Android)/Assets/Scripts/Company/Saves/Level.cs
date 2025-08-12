using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany.Save {

	[System.Serializable]
	public class Level {
		public int id;
		public bool isComplited;
		public int starCount;
		public List<Word> words;   // Слова


		public static Level CreateInstance() {
			Level lvl = new Level();
			lvl.words = new List<Word>();
			return lvl;
		}

	}
}