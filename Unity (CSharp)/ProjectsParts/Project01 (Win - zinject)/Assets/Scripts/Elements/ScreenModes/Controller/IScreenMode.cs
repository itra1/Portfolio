using Core.Elements.ScreenModes;

namespace Elements.ScreenModes.Controller
{
    public interface IScreenMode
    {
        ScreenMode CurrentMode { get; }
    }
}