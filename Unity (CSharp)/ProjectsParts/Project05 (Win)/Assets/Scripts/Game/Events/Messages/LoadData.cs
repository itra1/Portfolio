
using com.ootii.Collections;
using com.ootii.Messages;
using it.Game.Player.Save;

namespace it.Game.Events.Messages
{
  /// <summary>
  /// Событие запуска загрузки
  /// </summary>
  public class LoadData : Message
  {
	 private PlayerProgress _saveData;

	 public PlayerProgress SaveData { get => _saveData; set => _saveData = value; }

	 public override void Clear()
	 {
		base.Clear();

		SaveData = null;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is LoadData)
		{
		  sPool.Release(this);
		}
	 }


	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<LoadData> sPool = new ObjectPool<LoadData>(40, 10);

	 public new static LoadData Allocate()
	 {
		LoadData lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new LoadData(); }
		return lInstance;
	 }

	 public static void Release(LoadData rInstance)
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

		if (rInstance is LoadData)
		  sPool.Release((LoadData)rInstance);
	 }


  }
}