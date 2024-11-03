using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Guy : MonoBehaviour
{
    //public SpriteRenderer SpriteRenderer;
    public GameObject AcknowledgeObj;

    public AnimatedSprite Standing;
    public AnimatedSprite Walking;
    public AnimatedSprite Fixing;

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
    
    public IEnumerator GoFix(GlassTile glass, UnityAction onFixed)
    {
        IsFixing = true;
        var diff = glass.Coords.x - StandingOn.Coords.y + 2;
        var step = Mathf.Sign(diff);
        PlayAnim(Walking);
        Walking.SpriteRenderer.flipX = step > 0;
        for (var i = 0; i < Mathf.Abs(diff); i++)
        {
            yield return new WaitForSeconds(StepTime);
            var nextTile = step > 0 ? StandingOn.Next : StandingOn.Prev;
            transform.position = nextTile.transform.position + Vector3.up;
            StandingOn = nextTile;
        }
        PlayAnim(Fixing);
        yield return new WaitForSeconds(FixTime);
        glass.Fix();
        onFixed.Invoke();
        PlayAnim(Standing);
        IsFixing = false;
    }

    private void PlayAnim(AnimatedSprite anim)
    {
        Standing.gameObject.SetActive(anim == Standing);
        Walking.gameObject.SetActive(anim == Walking);
        Fixing.gameObject.SetActive(anim == Fixing);
        anim.PlayFromStart();
    }
}
