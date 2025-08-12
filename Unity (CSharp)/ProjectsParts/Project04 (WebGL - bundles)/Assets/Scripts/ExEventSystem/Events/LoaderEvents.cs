using UnityEngine;
using System.Collections;
using ExEvent;

public class LoaderEvents : MonoBehaviour {

	public sealed class OnWindowShow : BaseEvent {

		public GameObject type { get; private set; }

		public OnWindowShow(GameObject type) {
			this.type = type;
		}

		public static void Call(GameObject window) {
			Call(new OnWindowShow(window));
		}
	}

}
