using ExEvent;
using UnityEngine;

public class CameraController : Singleton<CameraController> {

	//public Transform parentCamera;
	public Camera cam { get; private set; }


	public float _rotationSpeed;
	public float _rotationSpeedMouse;
	public float _moveSpeed;
	public float _zoomSpeed;

	public float maxAngleVertical;
	public float minAngleVertical;

	public float maxZoom;
	public float minZoom;

	public float leftBorder;
	public float rightBorder;
	public float topborder;
	public float bottomBorder;

	private void OnEnable() {
		cam = GetComponent<Camera>();
	}

	private void Update() {
		GuiControl();
		KeyControl();

		if (_scroollButtonMove && Input.mousePosition != _lastMousePosition) {
			
			Vector3 oldPosition = transform.position;

			if (_isShift) {
				RotationHorizontalMouse(Input.mousePosition.x - _lastMousePosition.x);
				_lastMousePosition = Input.mousePosition;
			} else {
				Vector3 newPosition = GameManager.Instance.GetMapPosition(Input.mousePosition);
				if ((_lastCameraPosition - newPosition).magnitude > 30) return;
				oldPosition += _lastCameraPosition - newPosition;
				_lastMousePosition = Input.mousePosition;
			}
			oldPosition.x = Mathf.Clamp(oldPosition.x, leftBorder - 30, rightBorder + 30);
			oldPosition.z = Mathf.Clamp(oldPosition.z, bottomBorder - 30, topborder + 30);
			transform.position = oldPosition;
		}
	}

	public bool _isShift = false;
	public bool _scroollButtonMove;
	private Vector3 _lastCameraPosition;
	private Vector3 _lastMousePosition;


	private bool isShift {
		get { return _isShift; }
		set {
			_isShift = value;
			if (!_isShift) {
				_lastMousePosition = Input.mousePosition;
				_lastCameraPosition = GameManager.Instance.GetMapPosition(Input.mousePosition);
			}
		}
	}

	private void KeyControl() {

		if(isShift != Input.GetKey(KeyCode.LeftShift))
			isShift = Input.GetKey(KeyCode.LeftShift);

		if (Input.GetKey(KeyCode.W)) {
			forwardMove = 5;
		} else if (Input.GetKey(KeyCode.S)) {
			forwardMove = -5;
		} else {
			forwardMove = 0;
		}

		if (Input.GetKey(KeyCode.A)) {
			horizontalMove = -5;
		} else if (Input.GetKey(KeyCode.D)) {
			horizontalMove = 5;
		} else {
			horizontalMove = 0;
		}

		if (Input.GetKey(KeyCode.Q)) {
			horizontalRotate = -1;
		} else if (Input.GetKey(KeyCode.E)) {
			horizontalRotate = 1;
		} else {
			horizontalRotate = 0;
		}

		if (Input.GetKey(KeyCode.Z)) {
			verticalMove = -5;
		} else if (Input.GetKey(KeyCode.X)) {
			verticalMove = 5;
		} else {
			verticalMove = 0;
		}

		if (Input.GetKey(KeyCode.C)) {
      verticalRotate = 1;
    } else if (Input.GetKey(KeyCode.V)) {
      verticalRotate = -1;
		} else {
			verticalRotate = 0;
		}


		//forwardMove = Input.GetKey(KeyCode.W) ? 5 : 0;
		//forwardMove = Input.GetKey(KeyCode.X) ? -5 : 0;

		//horizontalMove = Input.GetKey(KeyCode.A) ? -5 : 0;
		//horizontalMove = Input.GetKey(KeyCode.D) ? 5 : 0;

		//horizontalRotate = Input.GetKey(KeyCode.Q) ? -1 : 0;
		//horizontalRotate = Input.GetKey(KeyCode.E) ? 1 : 0;

		//verticalMove = Input.GetKey(KeyCode.Z) ? -5 : 0;
		//verticalMove = Input.GetKey(KeyCode.X) ? 5 : 0;

		//verticalRotate = Input.GetKey(KeyCode.C) ? -1 : 0;
		//verticalRotate = Input.GetKey(KeyCode.V) ? 1 : 0;

	}

	//[ExEventHandler(typeof(GameEvents.KeyDown))]
	//public void KeyDown(GameEvents.KeyDown keyDown) {
	//	//if (keyDown.keyCode == KeyCode.LeftShift) {
	//	//	isShift = true;
	//	//}

	//	switch (keyDown.keyCode) {
	//		case KeyCode.LeftShift:
	//			isShift = true;
	//			break;
	//		case KeyCode.W:
	//			forwardMove = 5;
	//			break;
	//		case KeyCode.S:
	//			forwardMove = -5;
	//			break;
	//		case KeyCode.A:
	//			horizontalMove = -5;
	//			break;
	//		case KeyCode.D:
	//			horizontalMove = 5;
	//			break;
	//		case KeyCode.Q:
	//			horizontalRotate = -1;
	//			break;
	//		case KeyCode.E:
	//			horizontalRotate = 1;
	//			break;
	//		case KeyCode.Z:
	//			verticalMove = -5;
	//			break;
	//		case KeyCode.X:
	//			verticalMove = 5;
	//			break;
	//		case KeyCode.C:
	//			verticalRotate = -1;
	//			break;
	//		case KeyCode.V:
	//			verticalRotate = 1;
	//			break;
	//	}

	//}

	//[ExEventHandler(typeof(GameEvents.KeyUp))]
	//public void KeyUp(GameEvents.KeyUp keyUp) {
	//	switch (keyUp.keyCode) {
	//		case KeyCode.LeftShift:
	//			isShift = false;
	//			break;
	//		case KeyCode.W:
	//			forwardMove = 0;
	//			break;
	//		case KeyCode.S:
	//			forwardMove = 0;
	//			break;
	//		case KeyCode.A:
	//			horizontalMove = 0;
	//			break;
	//		case KeyCode.D:
	//			horizontalMove = 0;
	//			break;
	//		case KeyCode.Q:
	//			horizontalRotate = 0;
	//			break;
	//		case KeyCode.E:
	//			horizontalRotate = 0;
	//			break;
	//		case KeyCode.Z:
	//			verticalMove = 0;
	//			break;
	//		case KeyCode.X:
	//			verticalMove = 0;
	//			break;
	//		case KeyCode.C:
	//			verticalRotate = 0;
	//			break;
	//		case KeyCode.V:
	//			verticalRotate = 0;
	//			break;
	//	}
	//}

	public bool scroollButtonMove {
		set {
			_lastMousePosition = Input.mousePosition;
			_lastCameraPosition = GameManager.Instance.GetMapPosition(Input.mousePosition);
			_scroollButtonMove = value;
		}
	}

	public float forwardMove { get; set; }
	private float _lastForwardMove { get; set; }
	public float horizontalMove { get; set; }
	private float _lastHorizontalMove { get; set; }
	public float verticalMove { get; set; }
	private float _lastVerticalMove { get; set; }
	public float horizontalRotate { get; set; }
	private float _lastHorizontalRotate { get; set; }

	public float verticalRotate { get; set; }
	private float _lastVerticalRotate { get; set; }
	public float zoom { get; set; }
	private float _lastZoom { get; set; }

	private void GuiControl() {
		if (forwardMove != 0 || !(_lastForwardMove == 0 && forwardMove == _lastForwardMove)) {
			_lastForwardMove = forwardMove;
			MoveForward(forwardMove);
		}
		if (verticalMove != 0 || !(_lastVerticalMove == 0 && verticalMove == _lastVerticalMove)) {
			_lastVerticalMove = verticalMove;
			MoveVertical(verticalMove);
		}
		if (horizontalMove != 0 || !(_lastHorizontalMove == 0 && horizontalMove == _lastHorizontalMove)) {
			_lastHorizontalMove = horizontalMove;
			MoveHorizontal(horizontalMove);
		}

		if (verticalRotate != 0 || !(_lastVerticalRotate == 0 && verticalRotate == _lastVerticalRotate)) {
			_lastVerticalRotate = verticalRotate;
			RotatoinVertical(verticalRotate);
		}

		if (horizontalRotate != 0 || !(_lastHorizontalRotate == 0 && horizontalRotate == _lastHorizontalRotate)) {
			_lastHorizontalRotate = horizontalRotate;
			RotationHorizontal(horizontalRotate);
		}
		if (zoom != 0 || !(_lastZoom == 0 && zoom == _lastZoom)) {
			_lastZoom = zoom;
			Zooming(zoom);
		}
	}

	private Vector3 CheckShowCenter() {
		Vector3 pos = GameManager.Instance.GetMapPosition(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0)));
		//Physics.Raycast(transform.position, _camera.transform.forward, out hit, 1000);
		return pos;
	}

	private RaycastHit hit;
	public void MoveVertical(float value) {
		Vector3 newPosition = transform.position + transform.InverseTransformDirection(transform.up * _moveSpeed * value * Time.deltaTime);
		newPosition.y = Mathf.Clamp(newPosition.y, minZoom, maxZoom);
		transform.position = newPosition;
	}
	public void MoveForward(float value) {
		Vector3 newPosition = transform.position + new Vector3(transform.forward.x, 0, transform.forward.z).normalized * _moveSpeed * value * Time.deltaTime;

		if ((newPosition.x < leftBorder || rightBorder < newPosition.x) ||
				(newPosition.z < bottomBorder || topborder < newPosition.z)) return;

		//newPosition.x = Mathf.Clamp(newPosition.x, leftBorder, rightBorder);
		//newPosition.z = Mathf.Clamp(newPosition.z, bottomBorder, topborder);
		transform.position = newPosition;

	}
	public void MoveHorizontal(float value) {
		Vector3 newPosition = transform.position + transform.right * _moveSpeed * value * Time.deltaTime;
		if ((newPosition.x < leftBorder || rightBorder < newPosition.x) ||
				(newPosition.z < bottomBorder || topborder < newPosition.z)) return;

		//newPosition.x = Mathf.Clamp(newPosition.x, leftBorder, rightBorder);
		//newPosition.z = Mathf.Clamp(newPosition.z, bottomBorder, topborder);
		transform.position = newPosition;
	}

	public void RotatoinVertical(float value) {

		transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x + (_rotationSpeed * value), minAngleVertical, maxAngleVertical),
			transform.localEulerAngles.y,
			transform.localEulerAngles.z);

	}
	public void RotationHorizontal(float value) {
		try {
			transform.RotateAround(transform.position, Vector3.up, _rotationSpeed * value);
		} catch { }
	}
	public void RotationHorizontalMouse(float value) {
		try {

			//Vector3 centr = CheckShowCenter();
			//float X = transform.localEulerAngles.y + _rotationSpeedMouse * value;
			//Debug.Log(X);

			//transform.localEulerAngles = new Vector3(transform.localEulerAngles.y, X, transform.localEulerAngles.z);
			//Vector3 deltaDist = transform.position - centr;
			//transform.position = transform.localRotation * deltaDist + centr;

			transform.RotateAround(transform.position, Vector3.up, _rotationSpeedMouse * value);
		} catch { }
	}


	public void Scroll(float value) {
		if (_isShift) {
			RotatoinVertical(value);
		} else {
			Zooming(value);
		}
	}


	public void Zooming(float value) {

		Vector3 newPosition = transform.position + (CheckShowCenter() - transform.position).normalized * _zoomSpeed * value * Time.deltaTime;

		if (newPosition.y < minZoom || newPosition.y > maxZoom) return;

		transform.position = newPosition;
	}

}
