using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
	public class CatSceneEvent {
		
		public sealed class StartCatScene : BaseEvent {
			public string id;

			public StartCatScene(string id) {
				this.id = id;
			}

			public static void Call(string id) {
				BaseEvent.Call(new StartCatScene(id));
			}
		}

		public sealed class StartCatFrame : BaseEvent {
			public string id;
			public int pageNum;

			public StartCatFrame(string id, int pageNum) {
				this.id = id;
				this.pageNum = pageNum;
			}

			public static void Call(string id, int pageNum) {
				BaseEvent.Call(new StartCatFrame(id, pageNum));
			}
		}

		public sealed class EndCatFrame : BaseEvent {
			public string id;
			public int pageNum;

			public EndCatFrame(string id, int pageNum) {
				this.id = id;
				this.pageNum = pageNum;
			}

			public static void Call(string id, int pageNum) {
				BaseEvent.Call(new EndCatFrame(id, pageNum));
			}
		}

		public sealed class EndCatScene : BaseEvent {
			public string id;

			public EndCatScene(string id) {
				this.id = id;
			}

			public static void Call(string id) {
				BaseEvent.Call(new EndCatScene(id));
			}
		}
	}
}