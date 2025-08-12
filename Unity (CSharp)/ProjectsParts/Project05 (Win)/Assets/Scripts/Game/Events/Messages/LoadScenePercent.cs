
using com.ootii.Collections;
using com.ootii.Messages;

namespace it.Game.Events.Messages
{
  /// <summary>
  /// Прогресс загрузки сены
  /// </summary>
  public class LoadScenePercent : Message
  {
	 // FIX
	 private string _sceneName;
	 private float _progress;

	 public string SceneName { get => _sceneName; set => _sceneName = value; }
	 public float Progress { get => _progress; set => _progress = value; }

	 public override void Clear()
	 {
		base.Clear();

		// FIX
		SceneName = null;
		Progress = 0;
	 }

	 public override void Release()
	 {
		Clear();

		IsSent = true;
		IsHandled = true;

		if (this is LoadScenePercent)
		{
		  sPool.Release(this);
		}
	 }

	 // ******************************** OBJECT POOL ********************************

	 private static ObjectPool<LoadScenePercent> sPool = new ObjectPool<LoadScenePercent>(40, 10);

	 public new static LoadScenePercent Allocate()
	 {
		LoadScenePercent lInstance = sPool.Allocate();

		lInstance.IsSent = false;
		lInstance.IsHandled = false;

		if (lInstance == null) { lInstance = new LoadScenePercent(); }
		return lInstance;
	 }

	 public static void Release(LoadScenePercent rInstance)
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

		if (rInstance is LoadScenePercent)
		  sPool.Release((LoadScenePercent)rInstance);
	 }

  }
}