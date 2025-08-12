using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany {

	[System.Serializable]
	public class Word {
		public string word;
		public bool primary;
		public int starCount;
		public List<int> coinsLetters;

		public bool is_star {
			set {
				if (value)
					starCount = 1;
			}
		}

		public bool starExists {
			get { return starCount > 0; }
		}

		public List<Translation> translations;
	}

	[System.Serializable]
	public class Translation {
		public string lang;
		public string value;
	}

}