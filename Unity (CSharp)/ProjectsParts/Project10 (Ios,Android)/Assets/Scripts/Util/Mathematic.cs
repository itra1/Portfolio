using UnityEngine;
using System.Collections;

public struct ParabolaCoefficients {
	public float a;
	public float b;
	public float c;
}

[System.Serializable]
public struct sorterObjects {
	public GameObject obj;
	public float value;
}

// Класс с набором функция для математических рассчетов
public class Mathematic /*: MonoBehaviour*/  {

	public ParabolaCoefficients ParabolCoef;

	public void ParabolaCalcCoef(Vector2 frt, Vector2 scd, Vector2 trd) {
		/* Рассчет коеффицентов паработы по трем точкам
		 *          x3(y2-y1)+x2y1-x1y2
		 *     y3-_________________________
		 *                    x2-x1
		 * a = ____________________________
		 *           x3(x3-x1-x2)+x1x2
		 *           
		 *        y2-y1
		 * b = _________ - a(x1+x2)
		 *        x2-x1
		 *        
		 *     x2y1 - x1y2
		 * c = ____________ + ax1x2
		 *       x2-x1
		 */
		ParabolCoef = new ParabolaCoefficients();

		ParabolCoef.a = (trd.y - ((trd.x * (scd.y - frt.y) + scd.x * frt.y - frt.x * scd.y) / (scd.x - frt.x))) / (trd.x * (trd.x - frt.x - scd.x) + frt.x * scd.x);
		ParabolCoef.b = ((scd.y - frt.y) / (scd.x - frt.x)) - ParabolCoef.a * (frt.x + scd.x);
		ParabolCoef.c = (scd.x * frt.y - frt.x * scd.y) / (scd.x - frt.x) + (ParabolCoef.a * frt.x * scd.x);

	}

	// после инициализации ParabolCoef можно получить значение элементов
	public float ParabolaGetY(float X) {
		// y = aX^2+bX+c
		float Y = ParabolCoef.a * (X * X) + ParabolCoef.b * X + ParabolCoef.c;
		return Y;
	}

	public void ParabolaCalcCoef(Vector3 frt, Vector3 scd, Vector3 trd) {
		Vector2 start = new Vector2(frt.x, frt.y);
		Vector2 mid = new Vector2(scd.x, scd.y);
		Vector2 end = new Vector2(trd.x, trd.y);

		ParabolaCalcCoef(start, mid, end);
	}

	/* *************
	 * Инкремент массива для сортировки
	 * *************/
	public static void IncrementSorterObjects(ref sorterObjects[] arr, sorterObjects newObject) {
		var tempArr = new sorterObjects[arr.Length + 1];
		for (int i = 0; i < arr.Length; i++)
			tempArr[i] = arr[i];
		tempArr[arr.Length] = newObject;
		arr = tempArr;
		return;
	}

	/* *************
	 * Сортировка элементов массива
	 * *************/
	public static void SorterObjects(ref sorterObjects[] arr) {
		sorterObjects temp;

		for (int i = 0; i < arr.Length; i++) {
			// Массив просматривается с конца до
			// позиции i и "легкие элементы всплывают"
			for (int j = arr.Length - 1; j > i; j--) {
				// Если соседние элементы расположены
				// в неправильном порядке, то меняем
				// их местами
				if (arr[j].value < arr[j - 1].value) {
					temp = arr[j];
					arr[j] = arr[j - 1];
					arr[j - 1] = temp;
				}
			}
		}
	}
}
