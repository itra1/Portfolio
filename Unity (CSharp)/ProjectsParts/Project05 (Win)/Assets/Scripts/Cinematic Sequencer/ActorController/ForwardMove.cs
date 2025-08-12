using UnityEngine;
using System.Collections;
using com.ootii.Actors;

namespace Slate.ActionClips.ActorControllers
{
  [Category("Actor Controller")]
  [Description("Движение вперед на протчжении всего времени")]
  public class ForwardMove : ActorActionClip
  {

    [SerializeField]
    [HideInInspector]
    private float _length = 1;
    public EaseType interpolation = EaseType.QuadraticInOut;
    public float speed;

    private ActorController _actorController;
    private Vector3 _startPosition;

    //public override string info
    //{
    //  get { return string.Format("Change emition {0}", targetEmitions); }
    //}

    public override float length
    {
      get { return _length; }
      set { _length = value; }
    }
    protected override void OnEnter()
    {
      _actorController = actor.GetComponent<ActorController>();
      if (Application.isPlaying)
      {
        _startPosition = _actorController._Transform.position;
		}
		else
      {
        _startPosition = actor.transform.position;
      }

    }
    protected override void OnUpdate(float deltaTime)
    {

		if (Application.isPlaying)
      {
        var target = _startPosition + (_actorController._Transform.forward * speed * length);
        _actorController.SetVelocity(_actorController._Transform.forward * speed);
        //_actorController.SetPosition(Easing.Ease(interpolation, _startPosition, target, deltaTime / length));
      }
		else
      {
        var target = _startPosition + (actor.transform.forward * speed * length);
        actor.transform.position = Easing.Ease(interpolation, _startPosition, target, deltaTime / length);

      }


    }
	 protected override void OnExit()
    {
      if (Application.isPlaying)
      {
        _actorController.SetVelocity(Vector3.zero);
      }
		else
		{

		}
      }
  }
}