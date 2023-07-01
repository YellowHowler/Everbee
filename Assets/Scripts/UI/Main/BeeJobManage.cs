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
        kBee.UpdateJob(Job.Collect);
    }
    public void OnBuildJobBtnClick()
    {
        kBee.UpdateJob(Job.Build);
    }
    public void OnFeedJobBtnClick()
    {
        kBee.UpdateJob(Job.Feed);
    }
}
