using UnityEngine;

public class UUIDAttribute: PropertyAttribute {

  public bool drawnNewButton;

  public UUIDAttribute(bool drawnNewButton = true)
  {
	 this.drawnNewButton = drawnNewButton;
  }

}