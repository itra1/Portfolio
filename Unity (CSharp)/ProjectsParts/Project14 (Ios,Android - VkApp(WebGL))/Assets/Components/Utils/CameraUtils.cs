using UnityEngine;

namespace Itra.Utils {
	/// <summary>
	/// Ошибка типа камеры 
	/// </summary>
	public class CameraTypeException : System.ApplicationException {

		public CameraTypeException() : base() { }
		public CameraTypeException(string message) : base(message) { }

	}
	public static class CameraUtils {

		/// <summary>
		/// Видимая область ортографической камеры
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		/// <exception cref="CameraTypeException"></exception>
		public static Vector2 OrthographicVisibleRect(this Camera component) {
			if (!component.orthographic) {
				throw new CameraTypeException("Камера должна быть ортографической");
			}

			var screenProportion = (float)component.pixelRect.size.x / (float)component.pixelRect.size.y;
			return new Vector2(component.orthographicSize * screenProportion * 2, component.orthographicSize * 2);
		}

		/// <summary>
		/// Прямая видимая область перспективной камеры
		/// </summary>
		/// <param name="component"></param>
		/// <param name="distance">дистанция до обьекта</param>
		/// <returns></returns>
		/// <exception cref="CameraTypeException"></exception>
		public static Vector2 PerspectiveVisibleRect(this Camera component, float distance) {
			/*
			 * Берем прямоугольный треугольник как половину видимой области
			 * оприлагающая сторона от камеры рассчитывается по формуле
			 * с = H * cos(β)
			 * -β - противоположный угол
			 * 
			 * Половина видимой области:
			 * b = c/2
			 * 
			 */

			if (component.orthographic) {
				throw new CameraTypeException("Камера должна быть перспективной");
			}

			var screenProportion = (float)component.pixelRect.size.x / (float)component.pixelRect.size.y;
			var visibleSize = distance / Mathf.Cos((component.fieldOfView / 2) * Mathf.Deg2Rad);
			var baseSizeH = Mathf.Sqrt((visibleSize * visibleSize) - (distance * distance)) * 2;
			var baseSizeW = baseSizeH * screenProportion;
			return new Vector2(baseSizeW, baseSizeH);
		}

	}
}
