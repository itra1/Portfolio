using System.Collections;

using UnityEngine;

namespace it.Game.Environment.Level7.SarDayyni
{
  /// <summary>
  /// Контроллер щита
  /// </summary>
  public class SarDayynExtractorShield : MonoBehaviour
  {
	 public bool IsActived { get; set; }

	 private void Start()
	 {
		
	 }

	 public void SetACtivate(bool isActive)
	 {
		IsActived = isActive;



	 }


  }
}