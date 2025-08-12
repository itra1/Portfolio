using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputTest : MonoBehaviour {

  void Start() {

    string[] names = Input.GetJoystickNames();

    foreach(string nam in names) Debug.Log(nam);
        
  }
  
  void Update() {

    if(Input.GetKeyDown("joystick button 0")) {

    }

    if(Input.GetKeyDown("joystick button 1")) {

    }
    if(Input.GetKeyDown("joystick button 2")) {

    }
    if(Input.GetKeyDown("joystick button 3")) {

    }
    if(Input.GetKeyDown("joystick button 4")) {

    }
    if(Input.GetKeyDown("joystick button 5")) {

    }
    if(Input.GetKeyDown("joystick button 6")) {

    }
    if(Input.GetKeyDown("joystick button 7")) {

    }
    if(Input.GetKeyDown("joystick button 8")) {

    }
    if(Input.GetKeyDown("joystick button 9")) {

    }
    if(Input.GetKeyDown("joystick button 10")) {

    }
    if(Input.GetKeyDown("joystick button 11")) {

    }
    if(Input.GetKeyDown("joystick button 12")) {

    }
    if(Input.GetKeyDown("joystick button 13")) {

    }
    if(Input.GetKeyDown("joystick button 14")) {

    }
    if(Input.GetKeyDown("joystick button 15")) {

    }
    if(Input.GetKeyDown("joystick button 16")) {

    }
    if(Input.GetKeyDown("joystick button 17")) {

    }
    if(Input.GetKeyDown("joystick button 18")) {

    }
    if(Input.GetKeyDown("joystick button 19")) {

    }
    
  }
}
