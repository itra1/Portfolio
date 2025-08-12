using UnityEngine;

namespace Uuid {

	public class UUIDAttribute : PropertyAttribute {

		public bool EmptyOnlyEdit { get; private set; }
		public UUIDAttribute(bool emptyOnlyEdit = false) {
			EmptyOnlyEdit = emptyOnlyEdit;

		}

	}
}