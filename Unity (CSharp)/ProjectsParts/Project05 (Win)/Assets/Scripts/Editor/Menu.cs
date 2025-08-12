using UnityEngine;
using System.Collections;
using UnityEditor;
namespace it.Editor
{
#if UNITY_EDITOR
  public class Menu : MonoBehaviour
  {

	 [MenuItem("Tools/User/Clear PlayerPrefs")]
	 public static void ClearPlayerPrefs()
	 {
		PlayerPrefs.DeleteAll();
	 }

  }
#endif
}