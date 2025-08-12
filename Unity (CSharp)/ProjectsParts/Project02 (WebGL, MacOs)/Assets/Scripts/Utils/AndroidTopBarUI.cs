using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
public class AndroidTopBarUI : MonoBehaviour
{
    private float batteryStatus;
    [SerializeField]
    Slider BatterySlider;
   
    [SerializeField]
    TMP_Text Clocks;
    private float clockInvocationPeriod = 1f;
    private float batteryCheckInvocationPeriod = 60f;
    private bool isDoubleDots = true;
    private float WifiStatusInvocationPeriod = 60f;
   
    //variables for signal quality calculations
    int wifiInfo = 5;
    int Rssi;
    int dBm;
    int quality = 100;
    
    [SerializeField]
    Image[] Wifi;
    [SerializeField]
    Color WifiEnabledColor;
    [SerializeField]
    Color WifiDisabledColor;

    void Start()
    {
       
        InvokeRepeating("UpdateClocks", 0f, clockInvocationPeriod);
        InvokeRepeating("WifiStatus", 0f, WifiStatusInvocationPeriod);
        InvokeRepeating("BatteryStatus", 0f, batteryCheckInvocationPeriod);
           
    }


    void BatteryStatus()
    {
       
            batteryStatus = BatterySlider.value = SystemInfo.batteryLevel;
        
    }

    void UpdateClocks()
    {
        isDoubleDots = !isDoubleDots;
        if (isDoubleDots)
        {
            Clocks.text = String.Format("{0:d2}:{1:d2}", DateTime.Now.Hour, DateTime.Now.Minute);
        }
        else
        {
            Clocks.text = String.Format("{0:d2} {1:d2}", DateTime.Now.Hour, DateTime.Now.Minute);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("WifiStatus");
        CancelInvoke("BatteryStatus");
        CancelInvoke("UpdateClocks");
    }
    
    void WifiStatus()
    {

        //Network unavailable state This is Unity's own API to distinguish 4G wifi without internet 

        if (Application.internetReachability == NetworkReachability.NotReachable)

        {

            //A pop-up box prompts the user The network is disconnected; 

        }

        //When the user uses WiFi, the project requires detailed WiFi strength, so the source API of Android is connected 

        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)

        {

#if (UNITY_ANDROID) && !UNITY_EDITOR

            
            AndroidJavaObject UnityPlayerContext = new AndroidJavaObject("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            using (var wifiManager = UnityPlayerContext.Call<AndroidJavaObject>("getSystemService", "wifi"))
            {
                var connectionInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
                Rssi = connectionInfo.Call<int>("getRssi");               
            }

            int dBm = Rssi;
            int quality = 0;
            if (dBm <= -100)
                quality = 0;
            else if (dBm >= -50)
                quality = 100;
            else
                quality = 2 * (dBm + 100);
#endif

			wifiInfo = Mathf.RoundToInt( quality/100f * Wifi.Length);
            
           
            for (int a = 0; a < Wifi.Length; a++)
            {
                Wifi[a].color = WifiEnabledColor;
                if (a >= wifiInfo)
                {
                    Wifi[a].color = WifiDisabledColor;
                }
            }

            



        }

        //When the user uses the mobile network

        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) { }

    }
}
