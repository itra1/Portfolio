using System;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Map.Data
{
    [Serializable]
    public class Coordinate
    {
        [MaterialDataPropertyParse("lat")] 
        public double Y { get; set; }
        
        [MaterialDataPropertyParse("lng")] 
        public double X { get; set; }
    }
}