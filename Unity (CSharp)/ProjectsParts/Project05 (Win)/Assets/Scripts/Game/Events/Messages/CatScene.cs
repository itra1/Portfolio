using com.ootii.Collections;
using com.ootii.Messages;
using it.Game.Player.Save;

namespace it.Game.Events.Messages
{
  public class CatScene : Message
  {
	 public string Uuid { get => _uuid; set => _uuid = value; }
	 public StateType State { get => _state; set => _state = value; }

	 private string _uuid = "";

	 private StateType _state = StateType.start;

	 public enum StateType
	 {
		ready,
		start,
		complete
	 }
	 
	 public override void Clear()
	 {
		base.Clear();

		_uuid = null;
		_state = StateType.start;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is CatScene)
		{
		  sPool.Release(this);
		}
	 }

	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<CatScene> sPool = new ObjectPool<CatScene>(40, 10);

	 public new static CatScene Allocate()
	 {
		CatScene lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new CatScene(); }
		return lInstance;
	 }

	 public static void Release(CatScene rInstance)
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

		if (rInstance is CatScene)
		  sPool.Release((CatScene)rInstance);
	 }

  }
}