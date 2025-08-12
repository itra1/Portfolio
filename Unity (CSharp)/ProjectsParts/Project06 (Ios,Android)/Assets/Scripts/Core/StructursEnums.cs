/*
  Общие структуры и перечисления
*/

[System.Serializable]
/// <summary>
/// Диапазон float
/// </summary>
public struct SpanFloat {
    public float min;
    public float max;
}


[System.Serializable]
/// <summary>
/// Диапазон int
/// </summary>
public struct SpanInt {
    public int max;
    public int min;
}