using UnityEngine;
using System.Collections;

public class DontDestroyObject : MonoBehaviour
{
  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
}
