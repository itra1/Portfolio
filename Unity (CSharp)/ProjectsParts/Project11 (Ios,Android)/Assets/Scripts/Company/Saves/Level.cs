using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany.Save {

	[System.Serializable]
	public class Level {
		public int id;
		public bool isComplited;
    public List<Word> words = new List<Word>();
    
    public static Level CreateInstance() {
			Level lvl = new Level();
      lvl.words = new List<Word>();
			return lvl;
		}

	}
}