using UnityEngine;

public class SelectableSprite : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public bool Selectable { get; set; }
    
    public bool IsInside(Vector2 pos)
    {
        return SpriteRenderer.bounds.Contains(pos);
    }

    public virtual void Select()
    {
    }

    private void Reset()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
