/*
 * Контроллер элемента стены
 * 
 */

using UnityEngine;
using System.Collections;



public class WallController : MonoBehaviour {
    
    float speed; // Скорость смещения
    RunnerController runner;

    void OnEnable()
    {
        runner = GameObject.Find("GameController").GetComponent<RunnerController>();
    }

    void Update()
    {
        speed = runner.runSpeedActual * Time.deltaTime;
        transform.position = new Vector3(transform.position.x - speed*0.4f, transform.position.y,transform.position.z);
        
        if (transform.position.x <= 0) gameObject.SetActive(false);
    }
}
