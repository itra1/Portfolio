using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Transform")]
  [HutongGames.PlayMaker.Tooltip("Поиск позиции со смещение от целевого обьекти со смещение вперед")]
  public class GetPositionForwardOffset : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _transform;
	 public FsmVector3 _offset;
	 public FsmVector3 _position;


	 public override void OnEnter()
	 {
		UnityEngine.Vector3 position = _transform.Value.transform.position;

		position += _transform.Value.transform.forward * _offset.Value.z;
		position += _transform.Value.transform.up * _offset.Value.y;
		position += _transform.Value.transform.right * _offset.Value.x;


		_position.Value = position;
	 }

  }
}