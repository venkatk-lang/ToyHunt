using UnityEngine;
using System.Collections;
using System;

public enum MaskPlayMode
{
    Forward,
    Reverse
}

public class SpriteMaskAnimator : MonoBehaviour
{
    [Header("Mask Animation")]
    public SpriteMask mask;
    public Sprite[] frames;

    [Tooltip("Frames per second")]
    public float fps = 12f;
    public bool loop = true;

    private Coroutine playRoutine;
    private int index;
    private int direction = 1;

    float btweenDelay = 0f; 
    Action onComplete = null;
    private void Awake()
    {
        if (mask == null)
            mask = GetComponent<SpriteMask>();
    }

    // --------------------------------------------------
    // PUBLIC CONTROLS
    // --------------------------------------------------

    public void Play(MaskPlayMode playMode,float betweenDelay,Action OnComplete)
    {
        Stop();
        btweenDelay = betweenDelay;
        onComplete = OnComplete;
        switch (playMode)
        {
            case MaskPlayMode.Forward:
                direction = 1;
                index = 0;
                break;

            case MaskPlayMode.Reverse:
                direction = -1;
                index = frames.Length - 1;
                break;
        }

        playRoutine = StartCoroutine(PlayCoroutine(playMode));
    }
    public void Play(float betweenDelay, Action onMidComplete, Action onComplete)
    {
        Stop(); // stop any existing animation
        playRoutine = StartCoroutine(PlayForwardReverseCoroutine(betweenDelay, onMidComplete, onComplete));
    }
    private IEnumerator PlayForwardReverseCoroutine(float betweenDelay, Action onMidComplete,Action onComplete)
    {
        if (frames == null || frames.Length == 0)
            yield break;

        float delayPerFrame = 1f / fps;

        // --------------------
        // PLAY FORWARD
        // --------------------
        int index = 0;
        while (index < frames.Length)
        {
            mask.sprite = frames[index];
            yield return new WaitForSeconds(delayPerFrame);
            index++;
        }
        onMidComplete?.Invoke();
        // --------------------
        // BETWEEN DELAY
        // --------------------
        if (betweenDelay > 0f)
            yield return new WaitForSeconds(betweenDelay);

        // --------------------
        // PLAY REVERSE
        // --------------------
        index = frames.Length - 1;
        while (index >= 0)
        {
            mask.sprite = frames[index];
            yield return new WaitForSeconds(delayPerFrame);
            index--;
        }

        // --------------------
        // COMPLETE
        // --------------------
        onComplete?.Invoke();
    }
    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }

    public void SetFrame(int frame)
    {
        if (frames == null || frames.Length == 0) return;

        index = Mathf.Clamp(frame, 0, frames.Length - 1);
        mask.sprite = frames[index];
    }


    private IEnumerator PlayCoroutine(MaskPlayMode playMode)
    {
        if (frames == null || frames.Length == 0)
            yield break;

        float delay = 1f / fps;

        while (true)
        {
            mask.sprite = frames[index];
            yield return new WaitForSeconds(delay);

            index += direction;

            switch (playMode)
            {
                case MaskPlayMode.Forward:
                    if (index >= frames.Length)
                    {
                        if (loop) index = 0;
                        else yield break;
                    }
                    break;

                case MaskPlayMode.Reverse:
                    if (index < 0)
                    {
                        if (loop) index = frames.Length - 1;
                        else yield break;
                    }
                    break;
            }
        }
    }
}
