using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public struct InputStruct
{
    public bool Began;
    public bool Moved;
    public bool Ended;
    public Vector2 Position;
}

public static class Utility
{
    public static IEnumerator WaitUntilEvent(UnityEvent unityEvent) {
        var trigger = false;
        Action action = () => trigger = true;
        unityEvent.AddListener(action.Invoke);
        yield return new WaitUntil(()=>trigger);
        unityEvent.RemoveListener(action.Invoke);
    }

    public static IEnumerator WaitForClick()
    {
        var inputTime = 0f;
        var clicked = false;
        while (!clicked)
        {
            var input = GetInput();
            if (input.Began) inputTime = Time.time;
            if (input.Ended && Time.time - inputTime < .5f) clicked = true;

            yield return null;
        }
    }

    public static InputStruct GetInput()
    {
        InputStruct inputStruct;
        inputStruct.Began = Input.GetMouseButtonDown(0);
        inputStruct.Moved = Input.GetMouseButton(0);
        inputStruct.Ended = Input.GetMouseButtonUp(0);
        inputStruct.Position = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            var t = Input.touches[0];
            inputStruct.Began = t.phase == TouchPhase.Began;
            inputStruct.Moved = t.phase == TouchPhase.Moved;
            inputStruct.Ended = t.phase == TouchPhase.Ended;
            inputStruct.Position = t.position;
        }

        return inputStruct;
    }
}

public static class Extensions
{
    public static void Show(this CanvasGroup canvasGroup, float time = .2f)
    {
        canvasGroup.DOFade(1, time).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }
    
    public static void Hide(this CanvasGroup canvasGroup, float time = .2f)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, time);
    }

    public static Direction GetOpposite(this Direction direction)
    {
        switch (direction)
        {
            case Direction.S:
                return Direction.N;
            case Direction.W:
                return Direction.E;
            case Direction.N:
                return Direction.S;
            case Direction.E:
                return Direction.W;
        }

        return Direction.S;
    }
}