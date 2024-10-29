using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Guy : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public GameObject AcknowledgeObj;

    public Sprite Standing;
    public Sprite Walking;
    public Sprite Fixing;

    public FloorTile StandingOn;
    
    private const float StepTime = .3f;
    private const float FixTime = 2;

    public bool IsFixing;

    public IEnumerator Acknowledge()
    {
        AcknowledgeObj.SetActive(true);
        yield return new WaitForSeconds(.3f);
        AcknowledgeObj.SetActive(false);
        yield return new WaitForSeconds(.1f);
        AcknowledgeObj.SetActive(true);
        yield return new WaitForSeconds(.3f);
        AcknowledgeObj.SetActive(false);
    }
    
    public IEnumerator GoFix(GlassTile glass)
    {
        IsFixing = true;
        var diff = glass.Coords.y - StandingOn.Coords.y;
        var step = Mathf.Sign(diff);
        SpriteRenderer.sprite = Walking;
        SpriteRenderer.flipX = step < 0;
        for (var i = 0; i < Mathf.Abs(diff); i++)
        {
            yield return new WaitForSeconds(StepTime);
            var nextTile = step > 0 ? StandingOn.Next : StandingOn.Prev;
            transform.position = nextTile.transform.position + Vector3.up;
            StandingOn = nextTile;
        }
        SpriteRenderer.sprite = Fixing;
        yield return new WaitForSeconds(FixTime);
        glass.Fix();
        SpriteRenderer.sprite = Standing;
        IsFixing = false;
    }
}
