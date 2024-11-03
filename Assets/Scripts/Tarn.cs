using DG.Tweening;
using UnityEngine;

public class Tarn : MonoBehaviour
{
    public Transform Mask;
    public float MinY;

    private float _scale;
    private bool _reachedBottom;
    
    private void Start()
    {
        _scale = Mask.position.y - MinY;
    }

    public void Empty(float step)
    {
        if (_reachedBottom) return;
        
        var newPos = Mask.position.y - step * _scale;
        if (newPos < MinY)
        {
            _reachedBottom = true;
            newPos = MinY;
        }
        
        Mask.DOKill();
        Mask.DOMoveY(newPos, .3f);
    }
}
