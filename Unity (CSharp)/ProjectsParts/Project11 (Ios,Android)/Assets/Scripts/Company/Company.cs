using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCompany {

	[System.Serializable]
	public class Company {
		public string short_name;
		public CompanyLanuage lanuage;
		//public List<Location> locations;
		//public Location bonusLocation;
    public List<Level> levels;
    public List<Level> bonusLevels;
  }

}
public enum CompanyLanuage {
	none,
	en,
	ru
}