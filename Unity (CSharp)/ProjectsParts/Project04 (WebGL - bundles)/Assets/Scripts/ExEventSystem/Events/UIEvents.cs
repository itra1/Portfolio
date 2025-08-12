using System;
using UnityEngine;
using System.Collections;
using ExEvent;

public class UIEvents : MonoBehaviour {


    public sealed class ShowWindow : BaseEvent
    {

        public string window { get; private set; }

        public ShowWindow(string window)
        {
            this.window = window;
        }

        public static void Call(string window)
        {
            Call(new ShowWindow(window));
        }
    }

    public sealed class OnWindowShow : BaseEvent
    {

        public GameObject type { get; private set; }

        public OnWindowShow(GameObject type) {
            this.type = type;
        }

        public static void Call(GameObject window)
        {
            Call(new OnWindowShow(window));
        }
    }

    public sealed class OnWindowHide : BaseEvent
    {

        public GameObject type { get; private set; }

        public OnWindowHide(GameObject type)
        {
            this.type = type;
        }

        public static void Call(GameObject window)
        {
            Call(new OnWindowHide(window));
        }
    }

}
