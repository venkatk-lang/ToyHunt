using EasyTransition;
using System;
using System.Collections;
using UnityEngine;

public class TransitionWorldController : MonoBehaviour
{
    Action onTransitionBegin;
    Action onTransitionCutPointReached;
    Action onTransitionEnd;
    [SerializeField] GameObject transitionTemplate;
    Coroutine currentTransitionC;
    public void StartTransition(float startDelay,Action onBegin,Action onCover,Action onEnd)
    {
       
        if(currentTransitionC != null)
        {
            StopCoroutine(currentTransitionC);
        }
        currentTransitionC = StartCoroutine(Transition(startDelay));
        onTransitionBegin = null;
        onTransitionCutPointReached = null;
        onTransitionEnd = null;
        onTransitionBegin += onBegin;
        onTransitionCutPointReached += onCover;
        onTransitionEnd += onEnd;
    }
    IEnumerator Transition(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        onTransitionBegin?.Invoke();
        GameObject template = Instantiate(transitionTemplate) as GameObject;
        SimpleWorldTransition st = template.GetComponent<SimpleWorldTransition>();
        st.AnimateIn(1f, onTransitionCutPointReached, onTransitionEnd);
        currentTransitionC = null;
        yield break;
    }
   
}
