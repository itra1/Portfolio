using UnityEngine;
using System.Collections;

public class InteractionSetParent : MonoBehaviour
{
  public GameObject iObject;
  public GameObject hand;
  [ContextMenu("Set")]
  public void SetHand()
  {
    Vector3 position = hand.transform.position;
    Quaternion rotation = hand.transform.rotation;

    iObject.transform.position = hand.transform.position;
    var target = iObject.GetComponentInChildren<RootMotion.FinalIK.InteractionTarget>();
    hand.transform.position = target.transform.position;
    hand.transform.rotation = target.transform.rotation;
    iObject.transform.SetParent(hand.transform);
    hand.transform.position = position;
    hand.transform.rotation = rotation;
    hand.GetComponent<RootMotion.FinalIK.HandPoser>().poseRoot = target.transform;
  }
}
