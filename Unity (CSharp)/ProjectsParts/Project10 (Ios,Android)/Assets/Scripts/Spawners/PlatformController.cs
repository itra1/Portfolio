/*
 * Контроллер Платформы
 * 
 */

using UnityEngine;
using System.Collections;
public class PlatformController : MonoBehaviour
{
    float speed; // Скорость смещения
    RunnerController runner;

    void OnEnable()
    {
        runner = GameObject.Find("GameController").GetComponent<RunnerController>();
    }

    void Update()
    {
        speed = runner.runSpeedActual * Time.deltaTime;
        transform.position = new Vector2(transform.position.x - speed, transform.position.y);
        
        if (transform.position.x <= 0) gameObject.SetActive(false);
    }
}
