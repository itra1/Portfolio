using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Sprite avatarDefault;

    public static SpriteManager instance;

    private void Awake()
    {
        instance = this;
    }
}
