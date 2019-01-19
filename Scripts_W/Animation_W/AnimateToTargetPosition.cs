//copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel;

using MEC;

namespace Wichtel
{
public class AnimateToTargetPosition : MonoBehaviour
{
    private void OnEnable()
    {
        EventsAnimation.animateToTargetPosition += StartAnimation;
    }

    private void OnDisable()
    {
        EventsAnimation.animateToTargetPosition -= StartAnimation;
    }


    private void StartAnimation(Vector3 startPos, Vector3 endPos, float duration, Easings.Functions easingType, GameObject objectToAnimate)
    {
        //Stop previous coroutine, if it's animating the same object
        Timing.KillCoroutines(objectToAnimate.GetInstanceID(), "AnimateToTargetPosition");

        //Start animation coroutine
        Timing.RunCoroutine(_AnimationCoroutine(startPos, endPos, duration, easingType, objectToAnimate), objectToAnimate.GetInstanceID(), "AnimateToTargetPosition");
    }

    IEnumerator<float> _AnimationCoroutine(Vector3 startPos, Vector3 endPos, float duration, Easings.Functions easingType, GameObject objectToAnimate)
    {
        //Animation process
        for(float time = 0f; time < duration; time += Time.deltaTime)
        {
            //if gameObject exists
            if(objectToAnimate != null)
            {
                time = Mathf.Clamp(time, 0f, duration);
                float value = MathW.Remap(time, 0f, duration, 0f, 1f);

                Vector3 currentPosition = Vector3.LerpUnclamped(startPos, endPos, Easings.Interpolate(value, easingType));

                objectToAnimate.transform.position = currentPosition;

                yield return Timing.WaitForOneFrame;
            }

            //if gameObject was destroyed while being animated
            else
            {
                Debug.LogWarning("Animation Coroutine: objectToAnimate was destroyed");
                yield break;
            }

            //ensure that the final position is exactly the target position
            objectToAnimate.transform.position = endPos;
        }
    }
}
}
