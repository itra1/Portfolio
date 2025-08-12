using UnityEngine;



    public class ConsoleToGUI : MonoBehaviour
    {
#if UNITY_ANDROID 
        static string myLog = "";
        private string output;
        private string stack;

        private bool on = true;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            if (!Application.isEditor) 
            {
                if (on)
                {
                    GUI.Label(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
                }
                if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 120, 100, 50), on ? "OFF" : "ON"))
                {
                    on = !on;
                }
            }
        }
#endif
}
