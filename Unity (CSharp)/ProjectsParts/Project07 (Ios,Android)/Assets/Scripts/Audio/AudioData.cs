using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData {
	public AudioClip audioClip;
	[Range(0,1)]
	public float volume = 1;
	public bool loop = false;

	public FloatSpan pitch = new FloatSpan() {max = 1, min = 1 };

	[System.Serializable]
	public struct FloatSpan {
		public float max;
		public float min;
	}
}