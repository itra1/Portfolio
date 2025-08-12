using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany.Save {

	[System.Serializable]
	public class Company {
		public int id;
		public bool isComplited;
		public string shortCompany;
		public List<Location> locations;
		public Location bonusLocation;

		public static Company CreateForSave(Company source) {

			Company comp = new Company();
			comp.id = source.id;
			comp.isComplited = source.isComplited;
			comp.shortCompany = source.shortCompany;
			comp.bonusLocation = source.bonusLocation;
			//comp.starCount = source.starCount;

			return comp;

		}


	}
	

}