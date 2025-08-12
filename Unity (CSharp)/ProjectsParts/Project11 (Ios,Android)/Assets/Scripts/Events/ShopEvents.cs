using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
  public class ShopEvents {

    public sealed class OnProductBye: BaseEvent {

      public OnProductBye() {
      }

      public static void Call() {
        BaseEvent.Call(new OnProductBye());
      }

    }
  }
}