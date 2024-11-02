using DG.Tweening;
using UnityEngine;

public class Tarn : MonoBehaviour
{
    public Transform Mask;
    public float MinY;

    private float _scale;
    
    private void Start()
    {
        _scale = Mask.position.y - MinY;
    }

    public void Empty(float step)
    {
        Mask.DOKill();
        Mask.DOMoveY(Mask.position.y - step * _scale, .3f);
    }
}
