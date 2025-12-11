using UnityEngine;

public enum MaskPlayMode
{
    Forward,
    Reverse,
    PingPong
}

public class SpriteMaskAnimator : MonoBehaviour
{
    [Header("Mask Animation")]
    public SpriteMask mask;
    public Sprite[] frames;

    [Tooltip("Frames per second (speed).")]
    public float fps = 12f;

    [Tooltip("Playback mode.")]
    public MaskPlayMode playMode = MaskPlayMode.Forward;

    [Tooltip("Loop animation.")]
    public bool loop = true;

    private float timer;
    private int index;
    private bool isPlaying = false;
    private int direction = 1;   // +1 forward, -1 backward for reverse or pingpong

    private void Awake()
    {
        if (mask == null) mask = GetComponent<SpriteMask>();
    }

    private void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;
            StepFrame();
        }
    }

    private void StepFrame()
    {
        index += direction;

        switch (playMode)
        {
            case MaskPlayMode.Forward:
                if (index >= frames.Length)
                {
                    if (loop) index = 0;
                    else { isPlaying = false; index = frames.Length - 1; }
                }
                break;

            case MaskPlayMode.Reverse:
                if (index < 0)
                {
                    if (loop) index = frames.Length - 1;
                    else { isPlaying = false; index = 0; }
                }
                break;

            case MaskPlayMode.PingPong:
                if (index >= frames.Length)
                {
                    direction = -1;     // reverse direction
                    index = frames.Length - 2;
                }
                else if (index < 0)
                {
                    direction = 1;      // forward
                    index = 1;
                }
                break;
        }

        mask.sprite = frames[index];
    }

    // --------------------------------------------------
    // Public Controls
    // --------------------------------------------------

    public void Play()
    {
        isPlaying = true;
        timer = 0f;

        if (playMode == MaskPlayMode.Forward)
        {
            direction = 1;
            index = 0;
        }
        else if (playMode == MaskPlayMode.Reverse)
        {
            direction = -1;
            index = frames.Length - 1;
        }

        mask.sprite = frames[index];
    }

    public void PlayReverse()
    {
        playMode = MaskPlayMode.Reverse;
        direction = -1;
        index = frames.Length - 1;
        timer = 0f;
        isPlaying = true;

        mask.sprite = frames[index];
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void SetFrame(int frame)
    {
        if (frames.Length == 0) return;

        index = Mathf.Clamp(frame, 0, frames.Length - 1);
        mask.sprite = frames[index];
    }
}
