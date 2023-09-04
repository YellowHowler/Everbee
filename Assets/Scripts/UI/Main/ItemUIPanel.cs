using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIPanel : MonoBehaviour
{
    [HideInInspector] public bool isTrashcanHovered; 

    public void OnTrashcanHover()
    {
        isTrashcanHovered = true;
    }

    public void OnTrashcanHoverExit()
    {
        isTrashcanHovered = false;
    }
}
