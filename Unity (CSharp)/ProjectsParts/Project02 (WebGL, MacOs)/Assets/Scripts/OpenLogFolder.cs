using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using UnityEngine;

public class OpenLogFolder : MonoBehaviour
{
    public void OpenFolder()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo = new System.Diagnostics.ProcessStartInfo(Application.consoleLogPath);
        p.Start();
    }
}
