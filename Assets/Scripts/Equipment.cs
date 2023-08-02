using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Sprite _frontSprite;

    [SerializeField]
    private Sprite _backSprite;

    public void DisplayFront()
    {
        _spriteRenderer.sprite = _frontSprite;
    }

    public void DisplayBack()
    {
        _spriteRenderer.sprite = _backSprite;
    }
}
