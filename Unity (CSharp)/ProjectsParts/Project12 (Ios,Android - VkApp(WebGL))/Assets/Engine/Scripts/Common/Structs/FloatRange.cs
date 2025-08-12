namespace Engine.Scripts.Common.Structs
{
	[System.Serializable]
	public struct FloatRange
	{
		public float Max;
		public float Min;

		public float Ranmdom() => UnityEngine.Random.Range(Min, Max);
	}
}
