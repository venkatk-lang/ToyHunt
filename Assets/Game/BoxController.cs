using System;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public Transform boxRoot;
    public Vector3 upPosition;
    public Vector3 downPosition;
    public float moveTime = 0.4f;

    public void PlayInAnimation(Action onComplete = null)
    {
        // You can implement tweening here (LeanTween, DOTween) or simple coroutine.
        StartCoroutine(MoveCoroutine(downPosition, onComplete));
    }

    private System.Collections.IEnumerator MoveCoroutine(Vector3 target, Action onComplete)
    {
        float t = 0f;
        Vector3 start = boxRoot.localPosition;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            boxRoot.localPosition = Vector3.Lerp(start, target, t / moveTime);
            yield return null;
        }
        boxRoot.localPosition = target;
        onComplete?.Invoke();
    }
}
