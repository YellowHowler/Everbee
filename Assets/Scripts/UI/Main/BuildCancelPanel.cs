using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCancelPanel : MonoBehaviour
{
    public void SetBuildCancelBtnState(bool _isOver)
    {
        Mng.play.kHive.mMouseOverBuildCancel = _isOver;
    }
}
