using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Sprite[] Frames;

    public float FrameRate = 3f;

    private float _lastFrameTime;
    private int _lastFrameIdx;

    public void PlayFromStart()
    {
        gameObject.SetActive(true);
        _lastFrameIdx = 0;
        _lastFrameTime = -555;
    }
    
    private void Update()
    {
        if (gameObject.activeSelf && SpriteRenderer.color.a > 0)
        {
            if (Time.time - _lastFrameTime > 1 / FrameRate)
            {
                _lastFrameTime = Time.time;
                SpriteRenderer.sprite = Frames[_lastFrameIdx];
                ++_lastFrameIdx;
                _lastFrameIdx %= Frames.Length;
            }
        }
    }

    private void Reset()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
