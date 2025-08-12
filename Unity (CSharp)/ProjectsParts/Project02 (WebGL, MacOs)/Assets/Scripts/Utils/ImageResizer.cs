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
        // ����������� ����� ������ aspect ratio � ����� ������� � �������
        // ��� ����������� �������� ������������ �������� ����������� ��������� ���� if, ���� ��� ������� ������ ��� �������������� ����
        float preferredlWidth = parentWidth / spriteRenderer.sprite.bounds.size.x * aspectRatio;
        float preferredHeight = parentHeight / spriteRenderer.sprite.bounds.size.y;

        localTransform.localScale = new Vector2(preferredlWidth, preferredHeight);
    }
}
