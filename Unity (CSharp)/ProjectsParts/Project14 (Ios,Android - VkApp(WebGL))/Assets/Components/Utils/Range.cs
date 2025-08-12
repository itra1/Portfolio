namespace Utils {

	[System.Serializable]
	public struct FloatRange {
		public float Min;
		public float Max;
		public float Random() => UnityEngine.Random.Range(Min, Max);
	}

	[System.Serializable]
	public struct IntRange {
		public int Min;
		public int Max;

		public int Random(bool maxInclude = false) => UnityEngine.Random.Range(Min, Max + (maxInclude ? 1 : 0));
	}
}
