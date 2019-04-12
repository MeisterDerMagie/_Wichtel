using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wichtel.UI{
    [RequireComponent(typeof(ScrollRect))]
public class UI2008_LockScrollRectIfContentFitsViewport : MonoBehaviour
{
    private ScrollRect sr;
    bool sr_horizontalInitialValue, sr_verticalInitialValue;
    WaitForSeconds updateFrequency = new WaitForSeconds(0.5f);

    private void OnEnable()
    {
        sr = GetComponent<ScrollRect>();
        sr_horizontalInitialValue = sr.horizontal;
        sr_verticalInitialValue   = sr.vertical;

        StartCoroutine(CheckContentHeight());
    }

    private void OnDisable()
    {
        sr.horizontal = sr_horizontalInitialValue;
        sr.vertical   = sr_verticalInitialValue;
    }

    private void LockScrollRect()
    {
        sr.horizontal = false;
        sr.vertical   = false;
        sr.normalizedPosition = Vector2.zero;
    }

    private void UnlockScrollRect()
    {
        sr.horizontal = sr_horizontalInitialValue;
        sr.vertical   = sr_verticalInitialValue;
    }

    public IEnumerator CheckContentHeight()
    {
        for(;;)
        {
            if (sr.content.rect.height < sr.viewport.rect.height)
                LockScrollRect();
            else
                UnlockScrollRect();

            yield return updateFrequency;
        }
    }
}
}