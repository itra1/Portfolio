using UnityEngine;
using System.Collections;

/*
 * Класс отвечает за горизонтальное смещение объектов влево
 * */

public enum DeactiveType { destroy, deactive };

public class Mover : MonoBehaviour {

    RunnerController runner;                // Скорость
    public float speedKoef;                 // Коеффициент скорости
    float speed;                            // Скорость смещения

    public DeactiveType deactiveType = DeactiveType.destroy;

    void Start() {
        runner = GameObject.Find("GameController").GetComponent<RunnerController>();
    }

	void Update() {

        if (this.enabled == false) return;
        
        speed = runner.runSpeedActual * Time.deltaTime;
        transform.position = new Vector3(transform.position.x - speed * speedKoef, transform.position.y, transform.position.z);

        // Удалять при 0 позиции
        if (transform.position.x <= -10)
        {
            if (deactiveType == DeactiveType.destroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
	}
}
