using System;
using Elements.Presentation.Controller.CloneAlias.Data.Key;
using Elements.PresentationEpisodeScreen.Controller;
using Elements.Windows.Common.Controller;

namespace Elements.Presentation.Controller.CloneAlias.Data.Value
{
    public interface IWindowControllers : IDisposable
    {
        WindowAliasKey Key { get; }
        int Count { get; }
        IWindowController Owner { get; }
        
        bool IsEmpty();
        bool TryGetParent(IWindowController controller, out IPresentationEpisodeScreenInfo parent);
        bool TryGetNext(out IWindowController controller, out IPresentationEpisodeScreenInfo parent);
        bool TryAdd(IWindowController controller, IPresentationEpisodeScreenInfo parent);
        bool TryRemove(IWindowController controller);
    }
}