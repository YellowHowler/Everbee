using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeeJobManage : MonoBehaviour
{
    public Bee kBee { get; private set;}
    public Image kImage;
    public Toggle BuildButton, CollectButton, FeedButton;


    private void Update()
    {
        if (kBee != null)
        {
            kImage.sprite = kBee.GetCurrentSprite();

            SetBee(kBee);
        }
    }

    public void SetBee(Bee bee)
    {
        kBee = bee;

        BuildButton.isOn = (bee.kCurrentJob == Job.Build);
        if (BuildButton.isOn)
            BuildButton.Select();

        CollectButton.isOn = (bee.kCurrentJob == Job.Collect);
        if (CollectButton.isOn)
            CollectButton.Select();

        FeedButton.isOn = (bee.kCurrentJob == Job.Feed);
        if (FeedButton.isOn)
            FeedButton.Select();
    }

    public void OnCollectJobBtnChanged(bool val)
    {
        if (kBee != null)
        {
            if (CollectButton.isOn)
                kBee.UpdateJob(Job.Collect);
        }
    }
    public void OnBuildJobBtnChanged(bool val)
    {
        if (kBee != null)
        {
            if (BuildButton.isOn)
                kBee.UpdateJob(Job.Build);
        }
    }
    public void OnFeedJobBtnChanged(bool val)
    {
        if (kBee != null)
        {
            if (FeedButton.isOn)
                kBee.UpdateJob(Job.Feed);
        }
    }
}
