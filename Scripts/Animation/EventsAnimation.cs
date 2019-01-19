//copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel{
public class EventsAnimation : MonoBehaviour
{
    public static Action<Vector3/*StartPos*/, Vector3/*EndPos*/, float/*Duration*/, Wichtel.Easings.Functions/*EasingType*/, GameObject/*gameObjectToAnimate*/> animateToTargetPosition = delegate {};
}
}