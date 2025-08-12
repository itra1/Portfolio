using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombich.CollectionItems {

	public enum ItemType {
		bearMascotte
	}

	public class CollectionItemsManager : MonoBehaviour {
		public static CollectionItemsManager instance;

		List<CollectionItem> collectionList = new List<CollectionItem>();

		private void Awake() {

			if (instance != null) {
				Destroy(this);
				return;
			}

			instance = this;
	  }

	  private void Start()
	  {

	  }

    public void AddItem(ItemType type, Dictionary<string,object> data) {

			CollectionItem oneItem = new CollectionItem();
			oneItem.type = type;

			foreach (string dataKey in data.Keys) {
				if(dataKey == "winner")
					oneItem.isWinner = bool.Parse(data[dataKey].ToString());
			}
			
		}

		public List<CollectionItem> GetData() {
			return collectionList;
		}

		public void SetData(List<CollectionItem> data) {
			collectionList = data;
		}
				
	}
}