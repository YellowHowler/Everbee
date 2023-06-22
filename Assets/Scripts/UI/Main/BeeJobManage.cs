using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeJobManage : MonoBehaviour
{
    public Bee kBee;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollectJobBtnClick()
    {
        kBee.kCurrentJob = Job.Collect;
    }
    public void OnBuildJobBtnClick()
    {
        kBee.kCurrentJob = Job.Build;
    }
}
