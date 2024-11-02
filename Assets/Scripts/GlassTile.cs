using DG.Tweening;
using UnityEngine;

public class GlassTile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer WaterSpriteRenderer;
    public Sprite CrackedSprite;
    public Sprite CrackedLeakingSprite;
    public Sprite FixedSprite;
    
    public Vector2Int Coords;
    
    public bool Cracked;
    public bool Fixed;
    public bool Filled;
    
    public bool IsInside(Vector2 pos)
    {
        return SpriteRenderer.bounds.Contains(pos);
    }

    public void Fill(float time, float delay)
    {
        Filled = true;
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(1, time).SetDelay(delay).OnComplete(() =>
        {
            if (Cracked) SpriteRenderer.sprite = CrackedLeakingSprite;
        });
    }

    public void Empty(float time, float delay)
    {
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(0, time).SetDelay(delay).OnComplete(() =>
        {
            if (Cracked) SpriteRenderer.sprite = CrackedSprite;
        });
        Filled = false;
    }
    
    public void Crack()
    {
        SpriteRenderer.sprite = CrackedSprite;
        if (Filled) SpriteRenderer.sprite = CrackedLeakingSprite;
        Cracked = true;
        Fixed = false;
    }

    public void Fix()
    {
        SpriteRenderer.sprite = FixedSprite;
        Cracked = false;
        Fixed = true;
    }

    private void Reset()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
}
