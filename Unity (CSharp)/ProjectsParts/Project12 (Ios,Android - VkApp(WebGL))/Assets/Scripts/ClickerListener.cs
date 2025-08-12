using UnityEngine;
using UnityEngine.EventSystems;

public class ClickerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private Rigidbody2D _body;
	[SerializeField] private float _speed = 1;
	private bool _isDown;

	public void OnPointerDown(PointerEventData eventData)
	{
		_isDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isDown = false;
	}

	private void FixedUpdate()
	{
		if (_isDown)
			_body.AddForce(_speed * Time.deltaTime * Vector2.up, ForceMode2D.Force);
	}
}
