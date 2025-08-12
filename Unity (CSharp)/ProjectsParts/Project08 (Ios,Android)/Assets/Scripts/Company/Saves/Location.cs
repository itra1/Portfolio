using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany.Save {

	[System.Serializable]
	public class Location {
		public int id;
		public bool isOpen;
		public bool isFreePlay;
		public List<Level> levels;
		public int starCount;
	}
}