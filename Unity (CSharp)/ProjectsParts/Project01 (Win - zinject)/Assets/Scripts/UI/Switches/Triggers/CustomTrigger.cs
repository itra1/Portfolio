namespace UI.Switches.Triggers
{
    public class CustomTrigger : TriggerBase, ICustomTrigger
    {
        public CustomTrigger() : base(false) { }
        
        public void Enable()
        {
            Charged = true;
            Enabled = false;
            
            Fire();
        }
        
        public void Disable()
        {
            Charged = true;
            Enabled = true;
            
            Fire();
        }
        
        public void Reset()
        {
            Charged = false;
            Enabled = false;
        }
    }
}