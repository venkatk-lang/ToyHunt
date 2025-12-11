using UnityEngine;
namespace EasyTransition
{
    public class SimpleWorldTransition : MonoBehaviour
    {
        public TransitionSettings transitionSettings;

        public SpriteMaskAnimator maskSprite;

        bool hasTransitionTriggeredOnce;

        public void AnimateIn()
        {
            maskSprite.Play();
        }

        public void AnimateOut()
        {
            if (hasTransitionTriggeredOnce) return;

            hasTransitionTriggeredOnce = true;

            float destroyTime = transitionSettings.destroyTime;
            if (transitionSettings.autoAdjustTransitionTime)
                destroyTime = destroyTime / transitionSettings.transitionSpeed;

            Destroy(gameObject, destroyTime);
        }
    }

}