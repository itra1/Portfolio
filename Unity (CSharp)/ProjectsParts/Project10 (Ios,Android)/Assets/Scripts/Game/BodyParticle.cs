using UnityEngine;
using System.Collections;

public class BodyParticle : MonoBehaviour {

	float speed;                            // Скорость смещения
	float runSpeed;

	public DeactiveType deactiveType = DeactiveType.destroy;


	#region Move
	float angle;                                        // Угол поворота
	float speedXStart;                                  //
	float speedXNow;
	Vector3 velocity = Vector3.zero;                    // Рассчет смещения
	public float gravity;                               // Значение графитации
	bool first;                                         // Флаг первоначального косания поверхности
	#endregion

	#region Fixed
	public LayerMask groundMask;                        // Слой с поверхностью
	public float groundRadius;                          // Радиус определения поверхности
	bool fix = false;                                   // Флаг фиксации на замле
	#endregion

	#region Links
	public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки
	Mover mover;
	#endregion

	float rotateSeed;

	public float rotateKoef;

	void Start() {

#if UNITY_ANDROID
        LightTween.SpriteColorTo(graph.GetComponent<SpriteRenderer>() , new Color(1 , 1 , 1 , 0) , 0.5f);
#endif

		speedXStart = Random.Range(11, 15f) - RunnerController.RunSpeed;
		if (speedXStart == 0 && RunnerController.RunSpeed < 25)
			speedXStart = Random.Range(0, 2f);

		speedXNow = Mathf.Abs(speedXStart);
		first = true;
		rotateSeed = Random.Range(-1000, 1000);
		graph.transform.localPosition = new Vector3(graph.transform.localPosition.x, graph.transform.localPosition.y, Random.Range(graph.transform.localPosition.z - 0.2f, graph.transform.localPosition.z + 0.2f));
	}

	void OnEnable() {

	}

	public void SetStartCoef(float koefY) {
		velocity.y = Random.Range(-1 * koefY * 0.5f, 1 * koefY * 0.7f);
	}

	void Update() {
#if UNITY_IOS
		MovementIOS();
#endif
#if UNITY_ANDROID
        MoveAndroid();
#endif

		if (CameraController.displayDiff.leftDif(2) > transform.position.x || transform.position.y < 0) {
			if (deactiveType == DeactiveType.destroy)
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}
	}

#if UNITY_IOS

	Collider[] isGrounded;

	/// <summary>
	/// Движение при сборки под IOS
	/// </summary>
	void MovementIOS() {

		runSpeed = RunnerController.RunSpeed;

		if (fix) {
			speed = runSpeed * Time.deltaTime;
			transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);

			rotateSeed = (Mathf.Abs(rotateSeed) - (1500 * Time.deltaTime)) * Mathf.Sign(rotateSeed);

			angle += rotateSeed * Time.deltaTime;
			graph.transform.eulerAngles = new Vector3(0, 0, angle);
			return;
		}

		angle += rotateSeed * Time.deltaTime;
		graph.transform.eulerAngles = new Vector3(0, 0, angle);


		speedXNow -= (runSpeed + 8 / 3) * Time.deltaTime;
		velocity.x = speedXNow;

		if (velocity.x <= 0) {
			velocity.x = 0;
		} else
			velocity.x *= Mathf.Sign(speedXStart);


		velocity.y -= gravity * Time.deltaTime;


		isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
		if (isGrounded.Length > 0 && velocity.y < 0) {
			velocity.y = -velocity.y / 3f;

			if (velocity.x == 0 && !fix && velocity.y <= 1f) {
				fix = true;
			}

			if (first) {
				first = false;
				rotateSeed = -(Random.Range(0, 500) + runSpeed * rotateKoef);
				speedXNow = Random.Range(2f, 5f) - runSpeed;
				speedXNow -= (speedXNow / 3) + (runSpeed / 3);
				if (speedXNow <= 0) velocity.x = 0;
			}

		}

		transform.position += velocity * Time.deltaTime;
	}

#endif

#if UNITY_ANDROID

    /// <summary>
    /// Движение при сборки под Andrpod
    /// </summary>
    void MoveAndroid() {
        runSpeed = RunnerController.RunSpeed;
        
        angle += rotateSeed * Time.deltaTime;
        graph.transform.eulerAngles = new Vector3(0 , 0 , angle);
        
        speedXNow -= ( runSpeed + 8 / 3 ) * Time.deltaTime;
        velocity.x = speedXNow;

        if (velocity.x <= 0) {
            velocity.x = 0;
        } else
            velocity.x *= Mathf.Sign(speedXStart);


        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

#endif
}
