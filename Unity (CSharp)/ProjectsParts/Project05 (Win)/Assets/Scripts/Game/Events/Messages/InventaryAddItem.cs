using UnityEngine;
using com.ootii.Collections;
using com.ootii.Messages;
using System.Collections;

namespace it.Game.Events.Messages
{
  public class InventaryAddItem : Message
  {


	 public string Uuid { get => _uuid; set => _uuid = value; }
	 public int Count { get => _count; set => _count = value; }

	 private string _uuid;
	 private int _count;

	 public override void Clear()
	 {
		base.Clear();

		Uuid = null;
		Count = 0;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is InventaryAddItem)
		{
		  sPool.Release(this);
		}
	 }


	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<InventaryAddItem> sPool = new ObjectPool<InventaryAddItem>(40, 10);

	 public new static InventaryAddItem Allocate()
	 {
		InventaryAddItem lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new InventaryAddItem(); }
		return lInstance;
	 }

	 public static void Release(InventaryAddItem rInstance)
	 {
		if (rInstance == null) { return; }
		rInstance.Clear();

		rInstance.IsSent = true;
		rInstance.IsHandled = true;

		sPool.Release(rInstance);
	 }

	 public new static void Release(IMessage rInstance)
	 {
		if (rInstance == null) { return; }

		rInstance.Clear();

		rInstance.IsSent = true;
		rInstance.IsHandled = true;

		if (rInstance is InventaryAddItem)
		  sPool.Release((InventaryAddItem)rInstance);
	 }

  }
}