using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cells {

	[Serializable]
	public class Cell {

		public Action<Cell> OnChangeType;

		public int gridX;
		public int gridZ;
		/*
		public int gridX {
			get { return _gridX; }
			set { _gridX = value; }
		}

		public int gridZ {
			get { return MapBehaviour.Instance.cellLength - _gridZ; }
			set { _gridZ = value; }
		}
		*/
		public Vector3 position;
		public CellType type;

		public Cell() {
			position = new Vector3(0, 0, 0);
		}

		public Cell(float x, float z) {
			position = new Vector3(x, 0, z);
		}

		public bool EqualsRound(Cell target) {
			return Math.Round(position.x, 3) == Math.Round(target.position.x, 3) &&
			       Math.Round(position.z, 3) == Math.Round(target.position.z, 3);
		}

		public void SetType(CellType type) {
			this.type = type;
			if (OnChangeType != null) OnChangeType(this);
		}

		public static bool operator ==(Cell one, Cell two) {

			if ((object) two == null) {
				return (object) one == null;
			}

			return one.gridX == two.gridX && one.gridZ == two.gridZ;
		}

		public static bool operator !=(Cell one, Cell two) {
			if ((object) two == null) {
				return (object) one != null;
			}

			return one.gridX != two.gridX || one.gridZ != two.gridZ;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return int.Equals(gridX, ((Cell) obj).gridX) && int.Equals(gridZ, ((Cell) obj).gridZ);
		}

		public override int GetHashCode() {
			return gridX.GetHashCode() ^ gridZ.GetHashCode();
		}

	}
}

/// <summary>
	/// Типы ячеек
	/// </summary>
	public enum CellType {
		friendSelect = 0,    // перемещение
		moveReady = 1,     // Открыт
		close = 2,    // Занята
		enemySelect = 3,   // Не доступна
		my = 4,				// точка плеера
		friendLight = 5,		// Подсветка друзей
		enemyLight = 6,			// Посветка врагов
		magic = 7,     // Магия
		attack = 8     // Магия
}