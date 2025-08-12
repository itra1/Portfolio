
[System.Serializable]
public struct Vector3Light
{
  public float x;
  public float y;
  public float z;
}

[System.Serializable]
public struct RangeFloat
{
  public float Min;
  public float Max;

  public float RandomRange
  {
	 get {
		return UnityEngine.Random.Range(Min, Max);
	 }
  }
}

[System.Serializable]
public struct ItemLibrary
{
  public string name;
  public string path;
  public string uuid;
}
[System.Serializable]
public struct DataItem
{
  public string uuid;
  public int count;
}