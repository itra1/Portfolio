using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class UnderwaterCheck : MonoBehaviour
{

  public PostProcessVolume _underwaterProfile;
  public PostProcessVolume _upwaterProfile;
  public float waterheight = 0;

  private bool isUnderwater = false;

  private void Start()
  {
    SetUnderwater();
  }

  private void Update()
  {
    if(!isUnderwater && transform.position.y < waterheight)
    {
      isUnderwater = true;
      SetUnderwater();
    }
    if (isUnderwater && transform.position.y > waterheight)
    {
      isUnderwater = false;
      SetUnderwater();
    }
  }

  private void SetUnderwater()
  {
    _underwaterProfile.enabled = isUnderwater;
    _upwaterProfile.enabled = !isUnderwater;
  }

}
