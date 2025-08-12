using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageResizer : MonoBehaviour
{
    [SerializeField]
    Transform localTransform;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    SpriteMask parent;  
    void Start()
    {
        Resize();
    }

    public void Resize()
    {
        float parentWidth = parent.sprite.bounds.size.x;
        float parentHeight = parent.sprite.bounds.size.y;

        var aspectRatio = spriteRenderer.sprite.bounds.size.x / spriteRenderer.sprite.bounds.size.y;
        // опционально можно убрать aspect ratio и будет сжимать в квадрат
        // для правильного сжимания вертикальной картинки потребуется поставить один if, пока что оставил только для горизонтальных пикч
        float preferredlWidth = parentWidth / spriteRenderer.sprite.bounds.size.x * aspectRatio;
        float preferredHeight = parentHeight / spriteRenderer.sprite.bounds.size.y;

        localTransform.localScale = new Vector2(preferredlWidth, preferredHeight);
    }
}
