using Elements.Presentation.Controller.CloneAlias.Data.Key;
using Elements.PresentationEpisodeScreen.Controller;
using Elements.Windows.Base.Presenter;
using Elements.Windows.Common.Controller;

namespace Elements.Presentation.Controller.CloneAlias
{
    public interface IPresentationCloneAliasStorage
    {
        int CountBy(IWindowPresenter presenter);
        bool Contains(in WindowAliasKey key);
        bool Contains(IWindowController controller);
        bool ContainsOwner(in WindowAliasKey key);
        bool TryGetKey(IWindowController controller, out WindowAliasKey key);
        bool TryGetParent(IWindowController controller, out IPresentationEpisodeScreenInfo parent);
        bool TryGetOwner(in WindowAliasKey key, out IWindowController owner);
        bool TryGetNext(in WindowAliasKey key, out IWindowController controller, out IPresentationEpisodeScreenInfo parent);
        bool Add(IWindowController controller, IPresentationEpisodeScreenInfo parent, in WindowAliasKey key);
        bool Update(IWindowController controller, IPresentationEpisodeScreenInfo parent, in WindowAliasKey key);
        bool Remove(IWindowController controller);
        public void Clear();
    }
}