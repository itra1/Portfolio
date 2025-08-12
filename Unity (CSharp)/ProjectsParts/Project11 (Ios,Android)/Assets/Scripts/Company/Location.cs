using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany {

	[System.Serializable]
	public class Location {
		public int id;
		public string graphics_type;
		public string type;
		public int levelsCount;
		public string version;
    public LevelType locationType;
    public List<Level> levels;
    private bool _ischange;

		public bool isChange {
			get { return _ischange; }
			set {
				_ischange = value;
			}
		}
		

		public void ParseLevelType() {
			this.locationType = (LevelType)System.Enum.Parse(typeof(LevelType), this.type);
		}

	}

	public enum LevelType {
		free, bonus, inapp
	}

}