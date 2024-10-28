using DG.Tweening;
using UnityEngine;

public class GlassTile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer WaterSpriteRenderer;
    public Sprite DefaultSprite;
    public Sprite CrackedSprite;
    
    public Vector2Int Coords;
    
    public bool Cracked;
    public bool Filled;
    
    public bool IsInside(Vector2 pos)
    {
        return SpriteRenderer.bounds.Contains(pos);
    }

    public void Fill()
    {
        Filled = true;
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(1, .5f);
    }

    public void Empty()
    {
        WaterSpriteRenderer.DOKill();
        WaterSpriteRenderer.DOFade(0, .5f);
        Filled = false;
    }
    
    public void Crack()
    {
        SpriteRenderer.sprite = CrackedSprite;
        Cracked = true;
    }

    public void Fix()
    {
        SpriteRenderer.sprite = DefaultSprite;
        Cracked = false;
    }

    private void Reset()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        DefaultSprite = SpriteRenderer.sprite;
    }
    
}
