using DG.Tweening;
using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public Transform boxRoot;
    public Vector3 normalPos;
    public Vector3 outPos;
    public float moveTime = 1f;
    public IEnumerator PlayAnimation(Action onOutComplete, Action onComplete)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(boxRoot.DOLocalMove(outPos, 0.5f).SetEase(Ease.InBack));
        seq.AppendCallback(() => onOutComplete?.Invoke());
        seq.AppendInterval(1f);
        seq.Append(boxRoot.DOLocalMove(normalPos, 0.5f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => onComplete?.Invoke());
        yield return seq.WaitForCompletion();
    }
}
