using System.Collections.Generic;
using Game.Game.Elements.Interfaces;
using Game.Game.Settings;
using UnityEngine;

namespace Game.Runtime.Game.Elements.Boards
{
	public class BoardRecordFormation : MonoBehaviour
	{
		[SerializeField] private Transform _itemsParent;

		[ContextMenu("ReadFormatiion")]
		public List<BoardFormationItem> GetFormation()
		{
			var result = new List<BoardFormationItem>();
			var itemsArr = _itemsParent.GetComponentsInChildren<IFormationItem>();
			for (var i = 0; i < itemsArr.Length; i++)
			{
				var itm = itemsArr[i] as Component;
				result.Add(new BoardFormationItem()
				{
					Type = itemsArr[i].FormationType,
					LocalPosition = new Vector3(itm.transform.localPosition.x, itm.transform.localPosition.y, itm.transform.localPosition.z),
					LocalRotation = new Vector3(itm.transform.localEulerAngles.x, itm.transform.localEulerAngles.y, itm.transform.localEulerAngles.z),
					LocalScale = new Vector3(itm.transform.localScale.x, itm.transform.localScale.y, itm.transform.localScale.z)
				});
			}
			return result;
		}

	}
}
