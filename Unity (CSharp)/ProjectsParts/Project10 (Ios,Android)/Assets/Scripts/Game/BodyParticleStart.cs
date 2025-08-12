using UnityEngine;
using System.Collections;

public class BodyParticleStart : MonoBehaviour {
    

    #region Move
    float angle;                                        // Угол поворота
    Vector3 velocity = Vector3.zero;                    // Рассчет смещения
    public float gravity;                               // Значение графитации
    public GameObject paricles;
    #endregion
    
    public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки
    
    #region move
    bool isDown;                                        // Флаг, что объект опускается вниз
    #endregion

    float rotateSeed;
   
    void Start() {
        
        rotateSeed = Random.Range(-1000, 1000);
        graph.transform.localPosition = new Vector3(graph.transform.localPosition.x, graph.transform.localPosition.y, Random.Range(graph.transform.localPosition.z - 0.2f, graph.transform.localPosition.z + 0.2f));
    }

    public void SetStartCoef(float koefY) {
        velocity.y = Random.Range(-1 * koefY * 0.5f, 1 * koefY * 0.7f);
    }

    void Update() {
        // Движение
        Movement();

        if(CameraController.displayDiff.leftDif(2) > transform.position.x || transform.position.y < 0) {
            if(paricles != null){
                paricles.transform.parent = transform.parent;
                Destroy(paricles, 20);
            }
            Destroy(gameObject);
        }
            
    }


    void Movement() {
        
        angle += rotateSeed * Time.deltaTime;
        graph.transform.eulerAngles = new Vector3(0, 0, angle);
        
        velocity.x = 0;
        
        velocity.y -= gravity * Time.deltaTime;
        
        transform.position += velocity * Time.deltaTime;

    }

}