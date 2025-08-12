using System;
using UnityEngine;

namespace Settings.Data
{
    [Serializable]
    public struct WeatherData
    {
        public string Title;
        public string Key;
        public Sprite Icon;
    }
}