using UnityEngine;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;
using it.Game.Managers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  
  [MotionName("Form2Activator")]
  public class Form2Activator : MotionControllerMotion
  {

    public int _RenderObjectLayer = 1;
    private float _speedEnergyChange = 2;
    private float _priceEnergy = 10;
    public int RenderObjectLayer
    {
      get { return _RenderObjectLayer; }
      set { _RenderObjectLayer = value; }
    }

    private int _actualForm = 1;

    public Form2Activator() : base()
    {
      _Pack = Idle.GroupName();
      _Category = EnumMotionCategories.PLAYER;

      _Priority = 200;
      _ActionAlias = "ActivateForm";
      _OverrideLayers = true;

#if UNITY_EDITOR
      if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
    }

    public Form2Activator(MotionController rController) : base(rController)
    {
      _Pack = Idle.GroupName();
      _Category = EnumMotionCategories.PLAYER;

      _Priority = 200;
      _ActionAlias = "ActivateForm";

#if UNITY_EDITOR
      if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
    }

    private bool isActivate;

    public override bool TestActivate()
    {
      if (!mIsStartable) { return false; }
      if (!mMotionController.IsGrounded) { return false; }
      if (mActorController.State.Stance != EnumControllerStance.TRAVERSAL) { return false; }

      if (mMotionController.CurrentDress != 1)
        return false;

      if (_ActionAlias.Length > 0 && mMotionController._InputSource != null)
      {
        if (mMotionController._InputSource.IsJustPressed(_ActionAlias))
        {
          //if (mMotionController.CurrentForm != _actualForm &&
          //  GameManager.Instance.EnergyManager.Value < _priceEnergy)
          //  return false;

          return true;
        }
      }

      // Stop
      return false;
    }

    public override bool TestUpdate()
    {
      return true;
    }

    public override bool Activate(MotionControllerMotion rPrevMotion)
    {
      //int targetForm = mMotionController.CurrentForm == _actualForm ? 0 : _actualForm;

      PlayerSpecialChange(mMotionController.CurrentForm != _actualForm);


      //if (isActivate)
      //  AuraManager.Instance.ActivateForm2(() => { StateChange(); }, ()=> {
      //    Deactivate();
      //  }, 3, 0);
      //else
      //  PostProcessManager.Instance.PlayFullLight(() => { StateChange(); }, () => {
      //    Deactivate();
      //  }, 1, 0);


      //var dress = mMotionController.GetComponent<PlayerDress>();
      //    it.Game.Managers.PostProcessManager.Instance.PlayerForm2Process.weight = 0;
      //    mMotionController.CurrentForm = 1;
      //    dress.SetDress(1);

      return base.Activate(rPrevMotion);
    }

    public void PlayerSpecialChange(bool activateForm, UnityEngine.Events.UnityAction onMiddle = null)
	 {
      //if (activateForm)
      //  GameManager.Instance.EnergyManager.Subtract(_priceEnergy);

      //PlayerBehaviour.Instance.SetEnergyChange(activateForm, _speedEnergyChange);

      AuraManager.Instance.ActivateForm2(() => {
          onMiddle?.Invoke();
        StateChange(activateForm);
      }, () =>
        {
          Deactivate();
        }, 0.5f, 0);
    }

    public void StateChange(bool isActivate = false)
	 {
      var dress = mMotionController.GetComponent<PlayerDress>();

		if (isActivate)
		{
        //PostProcessManager.Instance.PlayerForm2Process.weight = 1;
        mMotionController.CurrentForm = _actualForm;
        //dress.SetDress(mMotionController.CurrentForm);
        //mMotionController.IsSpecial = true;
      }
		else
		{
        //PostProcessManager.Instance.PlayerForm2Process.weight = 0;
        mMotionController.CurrentForm = 0;
        AuraManager.Instance.DeactivateForm2();
        //dress.SetDress(LayerMask.NameToLayer("Default"));
        //mMotionController.IsSpecial = false;
      }


      //if (isActivate)
      //{

        //dress.SetLayer(Mathf.RoundToInt(Mathf.Log(RenderObjectLayer, 2)));
      //}
      //else
      //{

      //  it.Game.Managers.PostProcessManager.Instance.PlayerForm2Process.weight = 0;
      //  mMotionController.CurrentForm = 0;
      //  AuraManager.Instance.DeactivateForm2();
      //  dress.SetDress(LayerMask.NameToLayer("Default"));
      //  //dress.SetLayer(Mathf.RoundToInt(Mathf.Log(RenderObjectLayer, 2)));
      //}
      SendSaveMessage();
    }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {

		//Deactivate();

	 }


	 private void SendSaveMessage()
    {
      Game.Events.EventDispatcher.SendMessage(this, EventsConstants.PlayerFormChange, mMotionController.CurrentForm, 0);
    }

#if UNITY_EDITOR

    /// <summary>
    /// Allow the motion to render it's own GUI
    /// </summary>
    public override bool OnInspectorGUI()
    {
      bool lIsDirty = false;

      if (EditorHelper.TextField("Action Alias", "Action alias that starts the action and then exits the action (mostly for debugging).", ActionAlias, mMotionController))
      {
        lIsDirty = true;
        ActionAlias = EditorHelper.FieldStringValue;
      }

      // Balance layer
      int lNewRenderObjectLayer = EditorHelper.LayerMaskField(new GUIContent("Render Layer", ""), RenderObjectLayer);
      if (lNewRenderObjectLayer != RenderObjectLayer)
      {
        lIsDirty = true;
        RenderObjectLayer = lNewRenderObjectLayer;
      }

      return lIsDirty;
    }

#endif

  }
}