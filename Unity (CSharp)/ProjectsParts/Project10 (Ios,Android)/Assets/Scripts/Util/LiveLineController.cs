using UnityEngine;
using System.Collections;

public class LiveLineController : MonoBehaviour {

    public GameObject liveLine;                 //Сссылка на обхект полосы жизней;
    Vector3 defScale;                           // Стандартное значение жизней
    
    // Устанавливаем размер полосы жызней
    public void SetSize(float maxLive,float newLive) {

        // Устанавливаем стандартное значение
        if(defScale == Vector3.zero) defScale = liveLine.transform.localScale;
        
        float newScaleX = (defScale.x / maxLive) * newLive;

        // Новый размер
        liveLine.transform.localScale = new Vector3(newScaleX, defScale.y, defScale.z);
        
        float oneDiffX = 0.9f / maxLive;

        float diffX = -((0.9f - (oneDiffX* newLive)) / 2);

        liveLine.transform.localPosition = new Vector3(diffX,
                                                        liveLine.transform.localPosition.y, 
                                                        liveLine.transform.localPosition.z);
    }
}
