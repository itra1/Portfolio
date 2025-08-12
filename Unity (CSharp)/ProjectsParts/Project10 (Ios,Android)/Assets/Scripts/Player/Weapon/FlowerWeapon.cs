using UnityEngine;
using Spine.Unity;

public class FlowerWeapon : MonoBehaviour {
    
    SkeletonRenderer skeletonRenderer;
    
    public float speedX;                                // Скорость
    float angle;                                        // Угол поворота
    Vector3 velocity = Vector3.zero;                    // Рассчет смещения
    public float gravity;                               // Значение графитации
    
    public LayerMask groundMask;                        // Слой с поверхностью
    public float groundRadius;                          // Радиус определения поверхности
    
    public float dragTime;                              // Время сдерживание
    [HideInInspector]
    public bool act;                                    // Флаг активности, что может наносить повреждение
    
    public WeaponTypes thisWeaponType;                  // Текущий тип оружия
    public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки

    float rotationValue;

    bool shoot = false;

    void Start() {
        skeletonRenderer = Player.Jack.PlayerController.Instance.animation.skeletonRenderer;
        GetComponent<BoneFollower>().skeletonRenderer = skeletonRenderer;

        skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event += AnimEvent;
        act = true;

        rotationValue = 1000;

    }

    void Update() {

        if(shoot) {


            // Гравитация
            velocity.y -= gravity * Time.deltaTime;

            Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
            if(isGrounded.Length > 0 & velocity.y < 0) {
                velocity.y = 0;
                transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
            }

            // Вращение
            angle += rotationValue * Time.deltaTime;

            if((graph.transform.eulerAngles.z < 115 && angle >= 115) && isGrounded.Length > 0)
                angle = 115;


            if((graph.transform.eulerAngles.z < 260 && angle >= 260) && isGrounded.Length > 0)
                angle = 260;

            graph.transform.eulerAngles = new Vector3(0, 0, angle);

            // Скорость
            velocity.x = RunnerController.Instance.runSpeedActual;
            velocity.x += speedX;

            
            if((graph.transform.eulerAngles.z == 115 || graph.transform.eulerAngles.z == 260) && isGrounded.Length > 0) {
                velocity.x = 0;
                rotationValue = 0;
            }

            transform.position += velocity * Time.deltaTime;
        }
        
        //Уничтожение, при улетании объекта
        if(transform.position.x <= 0 || transform.position.y <= 0) {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy() {
        skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event -= AnimEvent;
    }

    void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
        GetComponent<BoneFollower>().enabled = false;

        transform.eulerAngles = new Vector3(0, 0, 0);
        angle = 180;
        graph.transform.eulerAngles = new Vector3(0, 0, angle);
        velocity.y = 10f;
        shoot = true;
    }

    void OnTriggerEnter(Collider col) {
        if(!act) return;
        
        if(col.tag == "Enemy") {
            col.GetComponent<EnemyMove>().dragTime = Time.time + dragTime;
            act = false;
            shoot = false;
        }
    }
}
