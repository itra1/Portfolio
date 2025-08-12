using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Camera.Data
{
    public class CameraDataBase : WidgetDataBase
    {
        [MaterialDataPropertyParse("camerasGrid")]
        public int[] CamerasGrid { get; set; }

        [MaterialDataPropertyParse("cameras")]
        public ulong?[] Cameras { get; set; }
    }
}