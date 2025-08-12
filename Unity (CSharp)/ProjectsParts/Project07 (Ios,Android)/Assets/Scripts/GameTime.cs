using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Переопределенный контроллер Time
/// </summary>
public class GameTime : MonoBehaviour {

  public static float time;
  public static float deltatime;
  public static float timeScale;

  private void Start() {
    timeScale = 1;
  }

  // Update is called once per frame
  void Update () {
    deltatime = Time.deltaTime;
    time += deltatime * timeScale;

  }
}
