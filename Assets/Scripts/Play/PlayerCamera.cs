using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    Transform mTarget;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(mTarget != null)
        {
            //Follow Code
        }        
    }

    public void SetFollow(Transform _target)
    {
        mTarget = _target;
    }
}
