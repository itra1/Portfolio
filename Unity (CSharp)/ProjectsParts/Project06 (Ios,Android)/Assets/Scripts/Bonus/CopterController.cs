using UnityEngine;

[System.Serializable]
public struct DeliverlyStruct
{
  public DeliveryType type;
  public GameObject pref;
  public Transform pointFly;

}


/// <summary>
/// Контроллер вертолета
/// </summary>
public class CopterController : MonoBehaviour {

  public static CopterController instance;
  public DeliverlyStruct[] delivery;

  void Awake()
  {
    instance = this;
    gameObject.SetActive(false);
  }

  void OnEnable()
  {
    deliverlyDropX = Random.Range(CameraController.leftPointX.x + 2f, CameraController.rightPoint.x - 8f);
  }

  void Start()    {
        InitMuve();
    }

    void Update()    {
    
        Muve();
        CheckBonus();
    }



    #region Muve
    Vector3 velocity;
    [SerializeField] float speedX;

    void InitMuve()    {
        velocity.x = speedX;

    }

    void Muve()    {
        transform.position += velocity * Time.deltaTime;
        if (transform.position.x > CameraController.rightPoint.x + CameraController.distanseX/2) gameObject.SetActive(false);
    }

  #endregion



  GameObject parcel;

    public GameObject SetTypeDeliverly(DeliveryType type)
  {
    transform.position = CameraController.leftTopPointWorldX + new Vector3(-CameraController.distanseX / 3f, -1f, 0f);
    for (int i = 0; i < delivery.Length; i++)
    {
      if (type == delivery[i].type)
      {
        parcel = (GameObject)Instantiate(delivery[i].pref, delivery[i].pointFly.position, Quaternion.identity);
        parcel.SetActive(true);
        parcel.transform.parent = transform;
        return parcel;
      }
    }
    return null;
  }


  #region Bonus
  float deliverlyDropX;




    void CheckBonus()
    {
        if (parcel == null) return;

        if(transform.position.x> deliverlyDropX)
        {
          parcel.transform.parent = transform.parent;
          if (parcel.GetComponent<PostController>()) parcel.GetComponent<PostController>().GoFree(speedX);
          if (parcel.GetComponent<MessageController>()) parcel.GetComponent<MessageController>().GoFree(speedX);
          parcel = null;
        }

    }

  #endregion


}
