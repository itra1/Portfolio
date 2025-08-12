using System;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;
using Core.Utils;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Widgets.EventsStat.Data
{
    public class Record : ISelfDeserializable
    {
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }
			
        [MaterialDataPropertyParse("value")]
        public int Value { get; set; }
			
        [MaterialDataPropertyParse("color")]
        public string ColorValue { get; set; }
			
        [MaterialDataPropertyParse("increment")]
        public int Increment { get; set; }
        
        public Color Color { get; private set; }
        
        public void Deserialize()
        {
            try
            {
                Color = ColorHelper.Parse(ColorValue);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Color = Color.white;
            }
        }
    }
}