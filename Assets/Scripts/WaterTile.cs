using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum WaterTileType
{
    Water, WaterTop, WaterEnd
}

public class WaterTile : MonoBehaviour
{
    public SpriteRenderer Water;
    public SpriteRenderer WaterTop;
    public SpriteRenderer WaterEnd;

    private const float MaxA = .65f;
    
    public void FadeIn(float time)
    {
        WaterEnd.DOKill();
        WaterEnd.DOFade(MaxA, time);
    }

    public void FilledNext(float time)
    {
        WaterEnd.DOKill();
        WaterTop.DOKill();
        WaterEnd.DOFade(0, 0);
        WaterTop.DOFade(MaxA, 0);
    }

    public void FilledTop(float time)
    {
        WaterTop.DOKill();
        Water.DOKill();
        WaterTop.DOFade(0, 0);
        Water.DOFade(MaxA, 0);
    }
    
    public void FadeOut(float time)
    {
        WaterEnd.DOKill();
        WaterTop.DOKill();
        WaterEnd.DOFade(0, time);
        WaterTop.DOFade(0, time);
    }
    
    public void EmptiedNext(float time)
    {
        WaterEnd.DOKill();
        WaterTop.DOKill();
        WaterEnd.DOFade(MaxA, 0);
        WaterTop.DOFade(0, 0);
    }

    public void EmptiedTop(float time)
    {
        WaterTop.DOKill();
        Water.DOKill();
        WaterTop.DOFade(MaxA, 0);
        Water.DOFade(0, 0);
    }
}
