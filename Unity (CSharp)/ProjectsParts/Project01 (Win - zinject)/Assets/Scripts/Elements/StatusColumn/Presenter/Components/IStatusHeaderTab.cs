using Base;
using UnityEngine;

namespace Elements.StatusColumn.Presenter.Components
{
    public interface IStatusHeaderTab : IRectTransformable, IUnloadable
    {
        Rect TitleRect { get; }
        bool IsBackgroundActive { get; }
        
        void SetBackgroundActive(bool value);
        void SetIconSprite(Sprite sprite);
        void SetTitleText(string text);
        void SetSeparatorActive(bool value);
    }
}