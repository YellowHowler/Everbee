using EnumDef;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeJobManage : MonoBehaviour
{
    public Bee kBee { get; private set;}
    public Image kImage;
    public TMP_Text kThinkingText;
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

        if (bee.mCurStage == BeeStage.Bee)
        {
            BuildButton.gameObject.SetActive(true);
            CollectButton.gameObject.SetActive(true);
            FeedButton.gameObject.SetActive(true);
            kThinkingText.gameObject.SetActive(true);

            BuildButton.isOn = (bee.kCurrentJob == Job.Build);
            if (BuildButton.isOn)
                BuildButton.Select();

            CollectButton.isOn = (bee.kCurrentJob == Job.Collect);
            if (CollectButton.isOn)
                CollectButton.Select();

            FeedButton.isOn = (bee.kCurrentJob == Job.Feed);
            if (FeedButton.isOn)
                FeedButton.Select();

            string thinkingText = "";
            switch(bee.Thinking)
            {
                case BeeThinking.None:                  thinkingText = ""; break;
                case BeeThinking.MovingToFlower:        thinkingText = "Moving to\r\nflower"; break;
                case BeeThinking.MovingToStorage:       thinkingText = "Moving to\r\nstorage"; break;
                case BeeThinking.MovingToLarvae:        thinkingText = "Moving to\r\nlarvae"; break;
                case BeeThinking.CollectingFromFlower:  thinkingText = "Collecting from\r\nflower"; break;
                case BeeThinking.Feeding:               thinkingText = "Feeding"; break;
                case BeeThinking.NoAvailableFlower:     thinkingText = "No available\r\nflower"; break;
                case BeeThinking.NoAvailableNectarStorage:  thinkingText = "No available\r\nnectar storage"; break;
                case BeeThinking.NoAvailablePollenStorage:  thinkingText = "No available\r\npollen storage"; break;
                case BeeThinking.NoAvailableHoneyStorage:   thinkingText = "No available\r\nhorney storage"; break;
                case BeeThinking.NoAvailableLarvae:     thinkingText = "No available\r\nlarvae"; break;
                case BeeThinking.NoPollenInThisStorage: thinkingText = "No pollen\r\nin this storage"; break;
                case BeeThinking.NoHoneyInThisStorage:  thinkingText = "No honey\r\nin this storage"; break;
            }

            if (kThinkingText.text != thinkingText)
                kThinkingText.text = thinkingText;
        }
        else
        {
            BuildButton.gameObject.SetActive(false);
            CollectButton.gameObject.SetActive(false);
            FeedButton.gameObject.SetActive(false);
            kThinkingText.gameObject.SetActive(true);

            kThinkingText.text = "";
        }
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
