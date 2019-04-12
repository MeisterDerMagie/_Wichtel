//------------------------------------------------------------------------------
// Copyright (c) 2014-2019 takomat GmbH and/or its licensors.
// All Rights Reserved.

// The coded instructions, statements, computer programs, and/or related material
// (collectively the "Data") in these files contain unpublished information
// proprietary to takomat GmbH and/or its licensors, which is protected by
// German federal copyright law and by international treaties.

// The Data may not be disclosed or distributed to third parties, in whole or in
// part, without the prior written consent of takomat GmbH ("takomat").

// THE DATA IS PROVIDED "AS IS" AND WITHOUT WARRANTY.
// ALL WARRANTIES ARE EXPRESSLY EXCLUDED AND DISCLAIMED. TAKOMAT MAKES NO
// WARRANTY OF ANY KIND WITH RESPECT TO THE DATA, EXPRESS, IMPLIED OR ARISING
// BY CUSTOM OR TRADE USAGE, AND DISCLAIMS ANY IMPLIED WARRANTIES OF TITLE,
// NON-INFRINGEMENT, MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE OR USE.
// WITHOUT LIMITING THE FOREGOING, TAKOMAT DOES NOT WARRANT THAT THE OPERATION
// OF THE DATA WILL BE UNINTERRUPTED OR ERROR FREE.

// IN NO EVENT SHALL TAKOMAT, ITS AFFILIATES, LICENSORS BE LIABLE FOR ANY LOSSES,
// DAMAGES OR EXPENSES OF ANY KIND (INCLUDING WITHOUT LIMITATION PUNITIVE OR
// MULTIPLE DAMAGES OR OTHER SPECIAL, DIRECT, INDIRECT, EXEMPLARY, INCIDENTAL,
// LOSS OF PROFITS, REVENUE OR DATA, COST OF COVER OR CONSEQUENTIAL LOSSES
// OR DAMAGES OF ANY KIND), HOWEVER CAUSED, AND REGARDLESS
// OF THE THEORY OF LIABILITY, WHETHER DERIVED FROM CONTRACT, TORT
// (INCLUDING, BUT NOT LIMITED TO, NEGLIGENCE), OR OTHERWISE,
// ARISING OUT OF OR RELATING TO THE DATA OR ITS USE OR ANY OTHER PERFORMANCE,
// WHETHER OR NOT TAKOMAT HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH LOSS
// OR DAMAGE.
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wichtel.UI{
public class UI_SetColorOfSpriteElement : MonoBehaviour
{
    [SerializeField]
    private Blueprint_Color color;

    public Blueprint_Color Color
    {
        get{ return color; }
        set{ color = value; SetColor(); }
    }

    private SpriteRenderer spriteRendererComponent;

    private void OnValidate()
    {
        SetColor();
    }

    private void SetColor()
    {
        if(GetComponent<SpriteRenderer>() != null)
        {
            spriteRendererComponent = GetComponent<SpriteRenderer>();

            if(color != null)
            {
                spriteRendererComponent.color = new Color( color.color.r, color.color.g, color.color.b, spriteRendererComponent.color.a );
            }
                
            else
            {
                Debug.LogWarning("<b><color=#dbce3a>"+"Missing color reference!"+"</color></b>", gameObject);
                spriteRendererComponent.color = new Color( 1f, 1f, 1f, spriteRendererComponent.color.a );
            }
                
        }
        else
        {
            Debug.LogWarning("<b><color=#dbce3a>"+"SetColorOfSpriteElement needs a SpriteRenderer Component!"+"</color></b>", gameObject);
        }
    }
}
}