using System;
using System.Collections;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform targetTransform;

    private readonly float Duration = UnoGameManager.WaitForOneMoveDuration;

    public void Move(Vector3 EndPosition, Action callback)
    {
        StartCoroutine(MoveToPosition(EndPosition, callback));
    }

    private IEnumerator MoveToPosition(Vector3 EndPosition, Action callBack)
    {
        float elapsedTime = 0;
        var start = targetTransform.position;
        while (elapsedTime < Duration)
        {
            targetTransform.position = Vector3.Lerp(start, EndPosition, elapsedTime / Duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = EndPosition;
        callBack();
    }
}