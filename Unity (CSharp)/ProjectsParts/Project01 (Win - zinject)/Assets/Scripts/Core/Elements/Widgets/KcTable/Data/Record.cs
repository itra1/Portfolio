using System;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;
using Core.Utils;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Widgets.KcTable.Data
{
    public class Record : ISelfDeserializable
    {
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }
        
        [MaterialDataPropertyParse("type")]
        public int Type { get; set; }
        
        [MaterialDataPropertyParse("authorName")]
        public string Author { get; set; }
        
        [MaterialDataPropertyParse("authorAVA")]
        public string AuthorAva { get; set; }
        
        [MaterialDataPropertyParse("status")]
        public string Status { get; set; }
        
        [MaterialDataPropertyParse("color")]
        public string ColorValue { get; set; }
        
        [MaterialDataPropertyParse("incident_number")]
        public int IncidentNumber { get; set; }
        
        public Color Color { get; private set; }
        
        public int StatusOrder { get; set; }
        
        public Record() => IncidentNumber = -1;

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