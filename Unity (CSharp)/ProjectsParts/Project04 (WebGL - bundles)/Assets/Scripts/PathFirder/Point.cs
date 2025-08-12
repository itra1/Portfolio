using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder {

	public class Point {

		/// <summary>
		/// Абсцисса точки "X"
		/// </summary>
		public float x;
		/// <summary>
		/// Свойство для чтения и записи абциссы точки
		/// </summary>
		public float X {
			get { return x; }
			set { x = value; }
		}
		/// <summary>
		/// Ордината точки "Y"
		/// </summary>
		public float y;
		/// <summary>
		/// Свойство для чтения и записи ординаты точки
		/// </summary>

		public float Y {
			get { return y; }
			set { y = value; }
		}
		/// <summary>
		/// Значание скаляра 
		/// </summary>
		private float value1;
		/// <summary>
		/// Свойство для чтения и записи значения скаляра
		/// </summary>
		public float Value1 {
			get { return value1; }
			set { value1 = value; }
		}

		/// <summary>
		/// Свойство, умножающее координаты точки на скаляр 
		/// </summary>
		public float Scalar {
			get { return value1; }
			set {
				x = x * value1;
				y = y * value1;
			}
		}
		/// <summary>
		/// Конструктор, создающий точку с заданными координатами и значение скаляра
		/// </summary>
		/// <param name="g"></param>
		/// <param name="h"></param>
		public Point(float g, float h, float r) {
			x = g;
			y = h;
			value1 = r;
		}
		/// <summary>
		/// Конструктор, создающий точку с нулевыми координатами
		/// </summary>
		public Point() {
			this.x = 0;
			this.y = 0;
		}

		public Point(float g, float h) {
			x = g;
			y = h;
		}

		/// <summary>
		/// Метод выведения координаты точки на экран
		/// </summary>
		public void Print() {
			Debug.Log(String.Format("Координата точки = ({0};{1})", x, y));
		}
		/// <summary>
		/// Метод выведения на экран расстояния от начала координат до точки
		/// </summary>
		/// <returns></returns>
		public float Distance() {
			return Mathf.Sqrt(x * x + y * y);
		}
		/// <summary>
		/// Метод перемещения точки на вектор (а,b)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public void Move(float a, float b) {
			x += a;
			y += b;
		}
		/// <summary>
		/// Индексатор,позволяющий по индексу 0 обращаться к полю x, по индексу 1 - к полю y.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public float this[int index] {
			get {
				if (index == 0) return x;
				if (index == 1) return y;
				else {
					throw new Exception();
				}
			}
			set {
				if (index == 0) x = value;
				if (index == 1) y = value;

			}
		}
		/// <summary>
		/// Одновременно увеличивает значение полей x и y
		/// </summary>
		/// <param name="ob"></param>
		/// <returns></returns>
		public static Point operator ++(Point ob) {
			ob.x = ob.x + 1;
			ob.y = ob.y + 1;
			return ob;
		}
		/// <summary>
		/// Одновременно уменьшает значение полей x и y
		/// </summary>
		/// <param name="ob"></param>
		/// <returns></returns>
		public static Point operator --(Point ob) {
			try {
				ob.x = ob.x - 1;
				ob.y = ob.y - 1;
				if (ob.y < 0 | ob.x < 0) throw new System.ArithmeticException();
			} catch (System.ArithmeticException) {
				Debug.Log("Уменьшение не возможно !");
				ob.x = ob.x + 1;
				ob.y = ob.y + 1;
			}
			return ob;

		}


		/// <summary>
		/// Возращает значение true, если поле x = y, иначе false
		/// </summary>
		/// <param name="ob"></param>
		/// <returns></returns>
		public static bool operator ==(Point x, Point y) {
			return (x.X == y.X) && (x.Y == y.Y);
		}

		/// <summary>
		/// Возращает значение true, если поле x != y, иначе false
		/// </summary>
		public static bool operator !=(Point x, Point y) {
			return (x.X != y.X) || (x.Y != y.Y);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return int.Equals(x, ((Point)obj).x) && int.Equals(y, ((Point)obj).y);
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode();
		}

		/// <summary>
		/// Добавляет к значению поля x и y значение скаляра n
		/// </summary>
		/// <param name="ob"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static Point operator +(Point ob, float n) {
			ob.x = ob.x + n;
			ob.y = ob.y + n;
			return ob;
		}

	}
}
