using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Input {

	[System.Serializable]
	public class ItemData : InputPackage {

		public string id;

		public string pid;
		public string item_id;
		public string status;
		public string place_id;
		public string type;
		public string name;
		public string img_preview;
		public string class_id;
		public string model;
		public string img1;
		public string img2;
		public string bind1_x;
		public string bind1_y;
		public string bind2_x;
		public string bind2_y;
	}
}