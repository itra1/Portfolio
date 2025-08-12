using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {

	/// <summary>
	/// Армия
	/// </summary>
	[System.Serializable]
	public class Army : InputPackage {

		public int? x;
		public int? y;

		public int posX {
			get { return (int)x; }
		}
		public int posY {
			get { return (int)y; }
		}
		public int army;
		public string login;
		public int complete;
		public int active;
	}

}
