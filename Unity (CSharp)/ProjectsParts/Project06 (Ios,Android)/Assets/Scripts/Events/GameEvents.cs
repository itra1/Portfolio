using UnityEngine;

namespace ExEvent {

  public class GameEvents {


    public sealed class KeyDown: BaseEvent {
      public KeyCode keyCode;

      public KeyDown(KeyCode keyCode) {
        this.keyCode = keyCode;
      }

      public static void Call(KeyCode keyCode) {
        BaseEvent.Call(new KeyDown(keyCode));
      }

    }


    public sealed class MouseScroll: BaseEvent {
      public float deltaScroll;

      public MouseScroll(float deltaScroll) {
        this.deltaScroll = deltaScroll;
      }

      public static void Call(float deltaScroll) {
        BaseEvent.Call(new MouseScroll(deltaScroll));
      }

    }

    public sealed class OnBye: BaseEvent {
      public ShopProductBehaviour product;


      public OnBye(ShopProductBehaviour product) {
        this.product = product;
      }

      public static void Call(ShopProductBehaviour product) {
        BaseEvent.Call(new OnBye(product));
      }

    }

    public sealed class GameDesignLoad: BaseEvent {

      public GameDesignLoad() {
      }

      public static void Call() {
        BaseEvent.Call(new GameDesignLoad());
      }
      public static void CallAsync()
      {
        BaseEvent.CallAsync(new GameDesignLoad());
      }

    }

  }

}
