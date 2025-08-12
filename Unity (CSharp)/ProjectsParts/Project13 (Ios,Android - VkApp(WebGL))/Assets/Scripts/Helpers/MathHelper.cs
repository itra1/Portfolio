
using UnityEngine;

public static class MathHelper
{
	public static Vector2 RotateVector(Vector2 v, float angle)
	{
		// Конвертируем угол из градусов в радианы
		float radian = angle * Mathf.Deg2Rad;

		// Рассчитываем новые координаты вектора после поворота
		float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
		float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);

		// Возвращаем новый повернутый вектор
		return new(_x, _y);
	}
}

[System.Serializable]
public struct RangeFloat
{
	public float Min;
	public float Max;

	public float Random => UnityEngine.Random.Range(Min, Max);
}
[System.Serializable]
public struct RangeInt
{
	public int Min;
	public int Max;
	public float RandomMaxExclusive => UnityEngine.Random.Range(Min, Max);
	public float RandomMaxInclusive => UnityEngine.Random.Range(Min, Max + 1);
}