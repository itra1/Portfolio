using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {

	/// <summary>
	/// Собственный персонаж
	/// </summary>
	[System.Serializable]
	public class MyPlayer : InputPackage {

		public int pid;
		public int? x;
		public int? y;
		public int? x1;
		public int? y1;
		public int killed;
		public int complete;
	}
}