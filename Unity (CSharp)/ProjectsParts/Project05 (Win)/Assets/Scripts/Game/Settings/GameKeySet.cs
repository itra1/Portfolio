using UnityEngine;
using System.Collections;
using com.ootii.Input;
using it.Game.Managers;
using com.ootii.Messages;

namespace it.Game.Settings
{
  public class GameKeySet : MonoBehaviourBase
  {
    [SerializeField]
    private bool pcGamePad = false;
    [SerializeField]
    private bool xBoxGamePad = true;

    private void Awake()
    {
      InputManager.Initialize();
      SetAliases();
    }

	 private void Start()
	 {

      Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
    }
    public void LoadHandler(IMessage handler)
    {
      SetAliases();
    }

    public void SetAliases()
    {
      //InputManager.RemoveAliases();
      InputManager.RemoveAliases();

      for (int i = 0; i < GameManager.Instance.GameKeys.Keys.Count; i++)
		{
        InputManager.AddAlias(GameManager.Instance.GameKeys.Keys[i].KeyAlias, GameManager.Instance.GameKeys.Keys[i].Value);
      }

      //InputManager.AddAlias("_MoveUpKey", EnumInput.W);
      //InputManager.AddAlias("_MoveDownKey", EnumInput.S);
      //InputManager.AddAlias("_MoveLeftKey", EnumInput.A);
      //InputManager.AddAlias("_MoveRightKey", EnumInput.D);
      //InputManager.AddAlias("_MoveLeftKey", EnumInput.LEFT_ARROW);
      //InputManager.AddAlias("_MoveRightKey", EnumInput.RIGHT_ARROW);
      //InputManager.AddAlias("_MoveUpKey", EnumInput.UP_ARROW);
      //InputManager.AddAlias("_MoveDownKey", EnumInput.DOWN_ARROW);
      InputManager.AddAlias("_MoveHorizontal", EnumInput.GAMEPAD_LEFT_STICK_X);
      InputManager.AddAlias("_MoveVertical", EnumInput.GAMEPAD_LEFT_STICK_Y);
      InputManager.AddAlias("_ViewHorizontal", EnumInput.GAMEPAD_RIGHT_STICK_X);
      InputManager.AddAlias("_ViewVertical", EnumInput.GAMEPAD_RIGHT_STICK_Y);
      InputManager.AddAlias("_MouseViewEnable", EnumInput.MOUSE_RIGHT_BUTTON);

      //// Прыжок
      //InputManager.AddAlias("Jump", EnumInput.SPACE);
      //if(xBoxGamePad)
      //  InputManager.AddAlias("Jump", EnumInput.GAMEPAD_0_BUTTON);
      //else
      //  InputManager.AddAlias("Jump", EnumInput.GAMEPAD_2_BUTTON);

      //// Интерактивность
      //InputManager.AddAlias("Interact", EnumInput.F);
      //if (xBoxGamePad)
      //  InputManager.AddAlias("Interact", EnumInput.GAMEPAD_2_BUTTON);
      //else
      //  InputManager.AddAlias("Interact", EnumInput.GAMEPAD_3_BUTTON);

      //// Инвентарь
      //InputManager.AddAlias("Inventary", EnumInput.I);

      //if (xBoxGamePad)
      //  InputManager.AddAlias("Inventary", EnumInput.GAMEPAD_3_BUTTON);
      //else
      //  InputManager.AddAlias("Inventary", EnumInput.GAMEPAD_0_BUTTON);

      //InputManager.AddAlias("PlayerDress1", EnumInput.ALPHA_1);
      //InputManager.AddAlias("PlayerDress2", EnumInput.ALPHA_2);
      //InputManager.AddAlias("PlayerDress3", EnumInput.ALPHA_3);

      //InputManager.AddAlias("ActivateFoem", EnumInput.T);

      ////if (xBoxGamePad)
      ////  InputManager.AddAlias("Form2", EnumInput.GAMEPAD_3_BUTTON);
      ////else
      ////  InputManager.AddAlias("Form2", EnumInput.GAMEPAD_0_BUTTON);

      //// Форма 2

      ////if (xBoxGamePad)
      ////  InputManager.AddAlias("Form2", EnumInput.GAMEPAD_3_BUTTON);
      ////else
      ////  InputManager.AddAlias("Form2", EnumInput.GAMEPAD_0_BUTTON);

      //InputManager.AddAlias("Run", EnumInput.LEFT_SHIFT);
      //InputManager.AddAlias("Run", EnumInput.GAMEPAD_LEFT_BUMPER);
      ////InputManager.AddAlias("Dress change", EnumInput.T);
      //InputManager.AddAlias("Move Down", EnumInput.Z);
      //InputManager.AddAlias("Move Up", EnumInput.X);
      //InputManager.AddAlias("Aiming", EnumInput.MOUSE_RIGHT_BUTTON);
      //InputManager.AddAlias("Attack", EnumInput.MOUSE_LEFT_BUTTON);
    }

  }
}