using System.Numerics;
using com.ootii.Collections;
using com.ootii.Messages;

namespace it.Game.Events.Messages
{
  public class SpawnPosition : Message
  {

	 private string _uuid;
	 private float _rotationY;

	 public string Uuid { get => _uuid; set => _uuid = value; }
	 public float RotationY { get => _rotationY; set => _rotationY = value; }

	 public override void Clear()
	 {
		base.Clear();

		Uuid = null;
		RotationY = 0;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is SpawnPosition)
		{
		  sPool.Release(this);
		}
	 }


	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<SpawnPosition> sPool = new ObjectPool<SpawnPosition>(40, 10);


	 public new static SpawnPosition Allocate()
	 {
		SpawnPosition lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new SpawnPosition(); }
		return lInstance;
	 }

	 public static void Release(SpawnPosition rInstance)
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

		if (rInstance is SpawnPosition)
		  sPool.Release((SpawnPosition)rInstance);
	 }


  }
}