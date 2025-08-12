using UnityEngine;
using com.ootii.Collections;
using com.ootii.Messages;
using System.Collections;

namespace it.Game.Events.Messages
{
  public class SymbolsAddItem : Message
  {

	 public string Uuid { get => _uuid; set => _uuid = value; }
	 private string _uuid;


	 public override void Clear()
	 {
		base.Clear();

		Uuid = null;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is SymbolsAddItem)
		{
		  sPool.Release(this);
		}
	 }


	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<SymbolsAddItem> sPool = new ObjectPool<SymbolsAddItem>(40, 10);

	 public new static SymbolsAddItem Allocate()
	 {
		SymbolsAddItem lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new SymbolsAddItem(); }
		return lInstance;
	 }

	 public static void Release(SymbolsAddItem rInstance)
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

		if (rInstance is SymbolsAddItem)
		  sPool.Release((SymbolsAddItem)rInstance);
	 }

  }
}