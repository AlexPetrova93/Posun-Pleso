using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guy : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Sprite Standing;
    public Sprite Walking;
    public Sprite Fixing;

    public FloorTile StandingOn;
    
    private const float StepTime = 1;
    
    public IEnumerator GoFix(GlassTile glass)
    {
        yield return new WaitForSeconds(StepTime);
    }
}
