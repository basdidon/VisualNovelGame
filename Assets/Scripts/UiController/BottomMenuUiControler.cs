using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BottomMenuUiControler : MonoBehaviour
{
    VisualElement root;

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement;
            root.style.marginTop = new StyleLength(StyleKeyword.Auto);
        }
    }
}
