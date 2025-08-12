using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombich.CollectionItems {

	/// <summary>
	/// Коллекционный предмет
	/// </summary>
	[System.Serializable]
	public class CollectionItem {

		public ItemType type;
		public bool isWinner;

		public void UseItem() {

		}

	}
}