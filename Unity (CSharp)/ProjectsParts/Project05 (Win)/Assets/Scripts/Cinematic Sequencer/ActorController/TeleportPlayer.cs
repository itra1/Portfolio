using UnityEngine;
using System.Collections;
using com.ootii.Actors;

namespace Slate.ActionClips.ActorControllers
{
  [Category("Actor Controller")]
  public class TeleportPlayer : ActorActionClip<ActorController>
  {
    public MiniTransformSpace space;
    public bool setPosition = true;
    public Vector3 position;

    public bool setRotation;
    public Vector3 rotation;


    protected override void OnEnter()
    {
		if (Application.isPlaying)
		{

        if (setPosition)
        {
          actor.SetPosition(TransformPosition(position, (TransformSpace)space));
        }

        if (setRotation)
        {
          actor.SetRotation(TransformRotation(rotation, (TransformSpace)space));
        }
        actor.SetRelativeVelocity(Vector3.zero);
        actor.SetVelocity(Vector3.zero);
		}
		else
		{
        if (setPosition)
          actor.transform.position = TransformPosition(position, (TransformSpace)space);
        if (setRotation)
          actor.transform.rotation = TransformRotation(rotation, (TransformSpace)space);
      }

    }

#if UNITY_EDITOR
    protected override void OnSceneGUI()
    {
      if (setPosition)
      {
        DoVectorPositionHandle((TransformSpace)space, ref position);
        if (setRotation)
        {
          DoVectorRotationHandle((TransformSpace)space, position, ref rotation);
        }
      }
    }
#endif
  }
}