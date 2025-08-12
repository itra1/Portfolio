using UnityEngine;
using System.Collections;

namespace it.Game.VideoScenes
{
  public class VideoScene : MonoBehaviour
  {
	 [SerializeField]
	 private string _title;

	 public string Title { get => _title; set => _title = value; }

	 public void Play()
	 {
		GetComponent<PlayMakerFSM>().SendEvent("StartFSM");
	 }
  }
}