using System;
using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;
using Core.Utils;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Widgets.Map.Data
{
    public class ExtendedLayerData : ISelfDeserializable
    {
        [MaterialDataPropertyParse("id")]
        public ulong Id { get; set; }
	    
        [MaterialDataPropertyParse("type")]
        public string Type { get; set; }
	    
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }
	    
        [MaterialDataPropertyParse("format")]
        public string Format { get; set; }
        
        [MaterialDataPropertyParse("colorMin")]
        public string MinColorValue { get; set; }
	    
        [MaterialDataPropertyParse("colorMax")]
        public string MaxColorValue { get; set; }
	    
        [MaterialDataPropertyParse("subtitle")]
        public string Subtitle { get; set; }
	    
        [MaterialDataPropertyParse("colorStep")]
        public int StepsCount { get; set; }
	    
        [MaterialDataPropertyParse("valueName")]
        public string ValueName { get; set; }
	    
        [MaterialDataPropertyParse("data")]
        public List<RegionData> Data { get; set; }
        
        public Color MinColor { get; private set; }
        public Color MaxColor { get; private set; }
        
        public void Deserialize()
        {
	        var defaultColor = Color.white;
	        
	        try
	        {
		        MinColor = string.IsNullOrEmpty(MinColorValue)
			        ? defaultColor
			        : ColorHelper.Parse(MinColorValue);
	        }
	        catch (Exception exception)
	        {
		        Debug.LogException(exception);
		        MinColor = defaultColor;
	        }
	        
	        try
	        {
		        MaxColor = string.IsNullOrEmpty(MaxColorValue)
			        ? defaultColor
			        : ColorHelper.Parse(MaxColorValue);
	        }
	        catch (Exception exception)
	        {
		        Debug.LogException(exception);
		        MaxColor = defaultColor;
	        }
        }
    }
}