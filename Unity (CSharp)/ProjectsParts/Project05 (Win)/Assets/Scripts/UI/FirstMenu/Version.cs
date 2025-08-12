using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace it.UI.FirtMenu
{
  public class Version : MonoBehaviour
  {
	 [SerializeField]
	 private Text _text;

	 private void Start()
	 {
		_text.text = "Ver.:" + Application.version;
	 }
  }
}