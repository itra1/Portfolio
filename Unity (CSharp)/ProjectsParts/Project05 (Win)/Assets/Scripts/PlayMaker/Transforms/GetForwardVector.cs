using HutongGames.PlayMaker;
using com.ootii.Cameras;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Transform")]
  public class GetForwardVector : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _transform;
	 public FsmVector3 _forvardVector;

	 public override void OnEnter()
	 {
		_forvardVector.Value = _transform.Value.transform.forward;
	 }

  }
}