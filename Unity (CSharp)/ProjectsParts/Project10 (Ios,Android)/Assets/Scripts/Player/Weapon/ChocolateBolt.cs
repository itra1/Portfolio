using UnityEngine;
using System.Collections;

public class ChocolateBolt : MonoBehaviour {


    public FloatSpan speedX;                            // Скорость
    float speedXact;
    Vector3 velocity = Vector3.zero;                    // Рассчет смещения
    public float gravity;                               // Значение графитации
    
    public LayerMask groundMask;                        // Слой с поверхностью
    public float groundRadius;                          // Радиус определения поверхности
    
    public float dragTime;                              // Время сдерживание
    [HideInInspector]
    public bool act;                                    // Флаг активности, что может наносить повреждение
    
    public WeaponTypes thisWeaponType;                  // Текущий тип оружия

    void Start () {
        act = true;
        speedXact = Random.Range(speedX.min, speedX.max);
    }
	
	// Update is called once per frame
	void Update () {

        velocity.y -= gravity * Time.deltaTime;
        Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
        if(isGrounded.Length > 0 & velocity.y < 0) {
            velocity.y = 0;
            transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
        }

        // Скорость
        velocity.x = speedXact;
        
        if(isGrounded.Length == 0)
        transform.position += velocity * Time.deltaTime;

    }

    void OnTriggerEnter(Collider col) {
        if(!act) return;

        if(col.tag == "Enemy") {
            if(col.GetComponent<EnemyMove>().dragTime < Time.time) {
                col.GetComponent<EnemyMove>().dragTime = Time.time + dragTime;
                act = false;
                Destroy(gameObject);
            }
        }
    }
}
