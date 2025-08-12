using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level6.Numbers
{
  /// <summary>
  /// Блок платформ
  /// </summary>
  public class PlatformsBlock : MonoBehaviour
  {
	 [SerializeField]
	 protected UnityEngine.Events.UnityEvent OnRoll;
	 [SerializeField]
	 private Platform[] _platforms;

	 private int _index;

	 /// <summary>
	 /// Кручение колеса
	 /// </summary>
	 public void WellRoll()
	 {
		OnRoll?.Invoke();
	 }

	 public void PlatformDown()
	 {
		_index++;

		_platforms[_index].SetHidePlatform(true);
	 }
  }
}