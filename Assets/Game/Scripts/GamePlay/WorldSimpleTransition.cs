using System;
using UnityEngine;
namespace EasyTransition
{
    public class SimpleWorldTransition : MonoBehaviour
    {
        public SpriteMaskAnimator maskSprite;

        public void AnimateIn(float delay,Action OnMidComplete, Action onComplete)
        {
            maskSprite.Play(delay, OnMidComplete, () => 
            {
                onComplete?.Invoke();
                Destroy(gameObject);
            });
        }

       
    }

}