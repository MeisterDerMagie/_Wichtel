using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wichtel;

public class AnimationBounce : MonoBehaviour
{
    private float value = 0f;
    private float time = 0f;
    private bool up = false;
    private Vector3 startPos = Vector3.zero;
    public Vector3 targetPos = new Vector3(0f, 360f, 0f);
    public float duration = 1f;
    public Easings.Functions wayUp = Wichtel.Easings.Functions.SineEaseInOut;
    public Easings.Functions wayDown = Wichtel.Easings.Functions.SineEaseInOut;
    private RectTransform ownRectTransform = null;
    private Vector3 currentPosition;

    public AnimationType animationType = AnimationType.Bounce;

    public enum AnimationType
	{
		Bounce,
		Repeat
    }
    

    private void Start()
    {
        //if it's a canvas object
        if(GetComponent<RectTransform>() != null)
        {
            ownRectTransform = GetComponent<RectTransform>();
            startPos = ownRectTransform.anchoredPosition;
        }
        //if it's a world object
        else
        {
            startPos = transform.position;
        }
    }

    private void Update()
    {        
        if(animationType == AnimationType.Bounce) BounceAnimation();
        else RepeatSingleAnimation();


        if(up)
        {
            currentPosition = Vector3.LerpUnclamped(startPos, targetPos, Easings.Interpolate(value, wayUp));
        }
        else
        {
            currentPosition = Vector3.LerpUnclamped(startPos, targetPos, Easings.Interpolate(value, wayDown));
        }

        if (ownRectTransform != null) ownRectTransform.anchoredPosition = currentPosition;
        else transform.position = currentPosition;
    }

    public void RepeatSingleAnimation()
    {
        if (time < duration)
        {
            time += Time.deltaTime;
        }
        else time = 0f;

        time = Mathf.Clamp(time, 0f, duration);
        value = MathW.Remap(time, 0f, duration, 0f, 1f);

    }

    public void BounceAnimation()
    {
        if (time < duration && up)
        {
            time += Time.deltaTime;
        }
        else if(time > 0 && !up)
        {
            time -= Time.deltaTime;            
        }        

        if (up && time >= duration)
        {
            up = false;
        }
        else if (!up && time <= 0f)
        {
            up = true;
        }
        

        time = Mathf.Clamp(time, 0f, duration);
        value = MathW.Remap(time, 0f, duration, 0f, 1f);
    }
}
