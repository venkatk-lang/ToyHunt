using EasyTransition;
using System;
using System.Collections;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    Action onTransitionBegin;
    Action onTransitionCutPointReached;
    Action onTransitionEnd;
    [SerializeField] GameObject transitionTemplate;
    Coroutine currentTransitionC;
    public void StartTransition(float startDelay, TransitionSettings transitionSettings,Action onBegin,Action onCover,Action onEnd)
    {
        if (transitionSettings == null) 
        {
            return;
        }
        if(currentTransitionC != null)
        {
            StopCoroutine(currentTransitionC);
        }
        currentTransitionC = StartCoroutine(Transition(startDelay, transitionSettings));
        onTransitionBegin += onBegin;
        onTransitionCutPointReached += onCover;
        onTransitionEnd += onEnd;
    }
    IEnumerator Transition(float startDelay, TransitionSettings transitionSettings)
    {
        yield return new WaitForSeconds(startDelay);
        onTransitionBegin?.Invoke();
        GameObject template = Instantiate(transitionTemplate) as GameObject;
        SimpleTransition st = template.GetComponent<SimpleTransition>();
        st.transitionSettings = transitionSettings;
        float transitionTime = transitionSettings.transitionTime;
        if (transitionSettings.autoAdjustTransitionTime)
            transitionTime = transitionTime / transitionSettings.transitionSpeed;
        st.AnimateIn();
        yield return new WaitForSeconds(transitionTime);
        onTransitionCutPointReached?.Invoke();
        st.AnimateOut();
        yield return new WaitForSeconds(transitionSettings.destroyTime);
        onTransitionEnd?.Invoke();
        onTransitionBegin = null;
        onTransitionCutPointReached = null;
        onTransitionEnd = null;
        currentTransitionC = null;
        yield break;
    }
   
}
