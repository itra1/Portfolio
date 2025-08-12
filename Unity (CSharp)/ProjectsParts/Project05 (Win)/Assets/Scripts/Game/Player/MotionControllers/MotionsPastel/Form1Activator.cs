using UnityEngine;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;
using it.Game.Managers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Form1Activator")]
  public class Form1Activator : MotionControllerMotion
  {

    public int _RenderObjectLayer = 1;
    public int RenderObjectLayer
    {
      get { return _RenderObjectLayer; }
      set { _RenderObjectLayer = value; }
    }

    public Form1Activator() : base()
    {
      _Pack = Idle.GroupName();
      _Category = EnumMotionCategories.PLAYER;

      _Priority = 199;
      _ActionAlias = "PlayerFrom1";
      _OverrideLayers = true;

#if UNITY_EDITOR
      if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
    }

    public Form1Activator(MotionController rController) : base(rController)
    {
      _Pack = Idle.GroupName();
      _Category = EnumMotionCategories.PLAYER;

      _Priority = 199;
      _ActionAlias = "PlayerFrom1";

#if UNITY_EDITOR
      if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
    }

    public override bool TestActivate()
    {
      if (!mIsStartable) { return false; }
      if (!mMotionController.IsGrounded) { return false; }
      if (mActorController.State.Stance != EnumControllerStance.TRAVERSAL) { return false; }
      if (mMotionController.CurrentDress == 0) { return false; }

      // Test if we're supposed to activate
      if (base._ActionAlias.Length > 0 && mMotionController._InputSource != null)
      {
        if (mMotionController._InputSource.IsJustPressed(base._ActionAlias))
        {
          return true;
        }
      }

      // Stop
      return false;
    }

    public override bool Activate(MotionControllerMotion rPrevMotion)
    {

		//if (mMotionController.IsSpecial)
		//{
  //      if(mMotionController.CurrentForm == 1)
		//  {

		//  }
		//}
      

      //AuraManager.Instance.ActivateForm2(() => { StateChange(); }, () =>
      //{
      //  Deactivate();
      //}, 3, 0);

      //var dress = mMotionController.GetComponent<PlayerDress>();

      //it.Game.Managers.PostProcessManager.Instance.PlayerForm2Process.weight = 0;
      //mMotionController.CurrentForm = 0;
      //dress.SetDress(0);


      //it.Game.Managers.PostProcessManager.Instance.PlayerForm2Process.weight = 1;


      //var dress = mMotionController.GetComponent<PlayerDress>();
      //mMotionController.CurrentForm = 0;
      //dress.SetDress(mMotionController.CurrentForm);
      //dress.SetLayer(Mathf.RoundToInt(Mathf.Log(RenderObjectLayer, 2)));

      //SendSaveMessage();
      // Return
      return base.Activate(rPrevMotion);
    }
    public void StateChange()
    {
  //    var dress = mMotionController.GetComponent<PlayerDress>();


		//PostProcessManager.Instance.PlayerForm2Process.weight = 0;
		//mMotionController.CurrentForm = 0;
		//AuraManager.Instance.DeactivateForm2();
		//dress.SetDress(LayerMask.NameToLayer("Default"));

		//SendSaveMessage();
    }
    public override void Update(float rDeltaTime, int rUpdateIndex)
    {

      Deactivate();

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