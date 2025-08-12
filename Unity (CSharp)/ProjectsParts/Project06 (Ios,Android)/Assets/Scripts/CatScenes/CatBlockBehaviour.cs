using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZbCatScene {
	public class CatBlockBehaviour : ScriptableObject {

		public bool helperStart;
		public bool helperEnd;

		public HeroType leftHero;
		public HeroType rightHero;

		public List<SoundQueue> soundQueue;

	}



	[System.Serializable]
	public class SoundQueue {
		public float time;
		public AudioBlock audioBlock;
	}
}