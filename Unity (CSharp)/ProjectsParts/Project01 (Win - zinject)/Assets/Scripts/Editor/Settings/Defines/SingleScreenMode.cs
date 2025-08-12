using Editor.Settings.Base;

namespace Editor.Settings.Defines
{
    public class SingleScreenMode : IToggleDefine
    {
        public string Symbol => "SINGLE_SCREEN";

        public string Description => "Single screen mode";
        
        public void AfterDisable() { }
		
        public void AfterEnable() { }
    }
}