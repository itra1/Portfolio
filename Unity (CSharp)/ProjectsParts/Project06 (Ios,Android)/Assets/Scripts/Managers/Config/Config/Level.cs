using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration {

	public class Level {

		public int chapter;
		public int level;
		public float? pointSize;
		public string mission;
		public int countFarm;
		public string title;
		public string description;
		public string brifing;
		public int fon;
		public int? openWeapon;
		public string tutor;
		public string enemy;
		public string survivle;
		public int drop1Id;
		public int drop1Count;
		public int drop2Id;
		public int drop2Count;
		public int drop3Id;
		public int drop3Count;
		public int coins;

		public bool isSurvivle {
			get { return !System.String.IsNullOrEmpty(survivle); }
		}

		public float mapLevelSize {
			get {
				if (pointSize.Value == 0)
          pointSize = 1;

				return pointSize.Value;
			}
		}

	}
}