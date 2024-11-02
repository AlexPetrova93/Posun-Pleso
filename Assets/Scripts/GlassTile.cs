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

    public void Fill()
    {
        Filled = true;
        if (Cracked) SpriteRenderer.sprite = CrackedLeakingSprite;
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(1, .5f);
    }

    public void Empty()
    {
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(0, .5f);
        if (Cracked) SpriteRenderer.sprite = CrackedSprite;
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
