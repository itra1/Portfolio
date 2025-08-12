using Core.Elements.Windows.Base.Data;
using Elements.FloatingWindow.Presenter.Factory;

namespace Elements.FloatingWindow.Controller.Factory
{
    public class FloatingWindowControllerFactory : IFloatingWindowControllerFactory
    {
        private readonly IFloatingWindowPresenterFactory _presenterFactory;
        
        public FloatingWindowControllerFactory(IFloatingWindowPresenterFactory presenterFactory) => 
            _presenterFactory = presenterFactory;
        
        public IFloatingWindowController Create(WindowMaterialData material, bool isAdaptiveSizeRequired) => 
            new FloatingWindowController(material, _presenterFactory, isAdaptiveSizeRequired);
    }
}
