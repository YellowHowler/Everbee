using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mng
{     
    static public SoundManager sound
    {
        get { return SoundManager.Instance; }
    }

    static public PlayManager play
    {
        get { return PlayManager.Instance; }
    }

    static public MainCanvas canvas
    {
        get { return MainCanvas.Instance; }
    }
}
