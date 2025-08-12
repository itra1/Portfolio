using com.ootii.Collections;
using com.ootii.Messages;
using it.Game.Player.Save;

namespace it.Game.Events.Messages.Player
{
  public class AnimEvent : Message
  {
	 private string _animEventName = "";

	 public string AnimEventName { get => _animEventName; set => _animEventName = value; }

	 public override void Clear()
	 {
		base.Clear();

		_animEventName = "";
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is AnimEvent)
		{
		  sPool.Release(this);
		}
	 }

	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<AnimEvent> sPool = new ObjectPool<AnimEvent>(40, 10);

	 public new static AnimEvent Allocate()
	 {
		AnimEvent lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new AnimEvent(); }
		return lInstance;
	 }

	 public static void Release(AnimEvent rInstance)
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

		if (rInstance is AnimEvent)
		  sPool.Release((AnimEvent)rInstance);
	 }
  }
}