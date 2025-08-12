using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExEvent {

  public class GameEvents: MonoBehaviour {

    public sealed class OnLoad: BaseEvent {

      public OnLoad() {
      }
      public static void Call() {
        BaseEvent.Call(new OnLoad());
      }
      public static void CallAsync() {
        BaseEvent.CallAsync(new OnLoad());
      }
    }

    public sealed class OnChangeEffects: BaseEvent {

      public OnChangeEffects() {
      }
      public static void Call() {
        BaseEvent.Call(new OnChangeEffects());
      }
      public static void CallAsync() {
        BaseEvent.CallAsync(new OnChangeEffects());
      }
    }

    public sealed class OnChangeMusic: BaseEvent {

      public OnChangeMusic() {
      }
      public static void Call() {
        BaseEvent.Call(new OnChangeMusic());
      }
      public static void CallAsync() {
        BaseEvent.CallAsync(new OnChangeMusic());
      }
    }

    public sealed class ChestShowComplete: BaseEvent {

      public ChestShowComplete( ) {
      }
      public static void Call() {
        BaseEvent.Call(new ChestShowComplete());
      }
      public static void CallAsync(bool isActive) {
        BaseEvent.CallAsync(new ChestShowComplete());
      }
    }



  }
}