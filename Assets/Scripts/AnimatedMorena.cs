using DG.Tweening;
using UnityEngine;

public class AnimatedMorena : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public int RequiredTile;

    public float MoveSpeed = .1f;
    
    private bool _visible;

    private Vector2 _minMaxY;
    
    private void Start()
    {
        SpriteRenderer.color = new Color(1, 1, 1, 0);
        _minMaxY = new Vector2(transform.position.y - 1, transform.position.y);
        MoveSpeed *= -1;
    }

    private void Update()
    {
        if (!_visible) return;

        var moveY = Time.deltaTime * MoveSpeed;
        if (transform.position.y + moveY < _minMaxY.x)
        {
            MoveSpeed *= -1;
        }
        if (transform.position.y + moveY > _minMaxY.y)
        {
            MoveSpeed *= -1;
        }

        transform.position += moveY*Vector3.up;
    }

    public void Appear()
    {
        if (_visible) return;
        
        _visible = true;
        SpriteRenderer.DOFade(1, .5f);
    }

    public void Disappear()
    {
        if (!_visible) return;
        
        _visible = false;
        SpriteRenderer.DOFade(0, .5f);
    }

    private void Reset()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
