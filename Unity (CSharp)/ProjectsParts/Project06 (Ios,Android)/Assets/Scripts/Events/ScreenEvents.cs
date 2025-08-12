using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
  public class ScreenEvents: MonoBehaviour {

    public sealed class PointerDown: BaseEvent {
      public Vector3 position;

      public PointerDown(Vector3 position) {
        this.position = position;
      }

      public static void Call(Vector3 position) {
        BaseEvent.Call(new PointerDown(position));
      }

    }

    public sealed class PointerUp: BaseEvent {
      public Vector3 position;

      public PointerUp(Vector3 position) {
        this.position = position;
      }

      public static void Call(Vector3 position) {
        BaseEvent.Call(new PointerUp(position));
      }

    }

    public sealed class PointerDrag: BaseEvent {
      public Vector3 delta;
      public Vector3 position;

      public PointerDrag(Vector3 position, Vector3 delta) {
        this.position = position;
        this.delta = delta;
      }

      public static void Call(Vector3 position, Vector3 delta) {
        BaseEvent.Call(new PointerDrag(position, delta));
      }

    }

    public sealed class Scroll: BaseEvent {
      public float delta;

      public Scroll(float delta) {
        this.delta = delta;
      }

      public static void Call(float delta) {
        BaseEvent.Call(new Scroll(delta));
      }

    }


  }
}