using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveItem : MonoBehaviour
{
  [SerializeField]
  private Image image;

  private Transform targetPosition;

  private float maxDistance;

  private float moveSpeed = 15f;

  public void SetMove(Image sourceImage, Transform targetPosition)
  {
    this.targetPosition = targetPosition;
    this.image.sprite = sourceImage.sprite;
    this.image.rectTransform.sizeDelta = new Vector2(sourceImage.rectTransform.rect.width, sourceImage.rectTransform.rect.height);
    transform.position = sourceImage.transform.position;
    transform.localScale = Vector3.one;
    maxDistance = (targetPosition.position - transform.position).magnitude;
  }

  private void Update()
  {
    Vector3 newPosition = transform.position + (targetPosition.position - transform.position).normalized * Time.deltaTime * moveSpeed;

    float newDitance = (targetPosition.position - newPosition).magnitude;
    transform.localScale = Vector3.one * (newDitance/ maxDistance);

    if ((targetPosition.position - transform.position).magnitude < (newPosition - transform.position).magnitude)
    {
      gameObject.SetActive(false);
      UserManager.Instance.ConfirmAction();
    }
    else
    {
      transform.position = newPosition;
    }

  }




}
