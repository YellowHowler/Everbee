using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceConvertPanel : MonoBehaviour
{
    public Image kBackground;
    public Sprite[] kBackgroundSprites;

    private float mConvertProgress = 0; 
    private float mTotConvertTime = 20;
    private int mConvertSpriteLength = 24;
    private float mConvertStepSec;

    public Image kPrevImage;
    public TMP_Text kPrevText;

    [HideInInspector] public GameResAmount mPrevAmount = new GameResAmount(0f, GameResUnit.Microgram);
    [HideInInspector] public GameResType mPrevType = GameResType.Empty;

    [HideInInspector] public bool mIsPrevHover;

    public Image kResImage;
    public TMP_Text kResText;

    [HideInInspector] public GameResAmount mResAmount = new GameResAmount(0f, GameResUnit.Microgram);
    [HideInInspector] public GameResType mResType = GameResType.Empty;

    private bool mIsResFull;
    private bool mIsConverting;

    private void Start()
    {
    }

    public void OnPrevHover()
    {
        mIsPrevHover = true;
    }

    public void OnPrevHoverExit()
    {
        mIsPrevHover = false;
    }

    public void OnPrevClick()
    {
        if(Mng.play.kHive.mIsPlacingItem == true) 
            return;

        if(Mng.play.IsAmountZero(mPrevAmount)) return;

        Mng.canvas.SpawnItemAtMousePos(mPrevType, mPrevAmount, ItemLoc.ConvertPrevSlot, -1);
        UpdatePrevAmount(GameResType.Empty, new GameResAmount(0f, GameResUnit.Microgram));
    }

    public void OnResClick()
    {
        if(Mng.play.kHive.mIsPlacingItem == false) 
            EndConvert();
        else   
            return;
       

        if(mIsResFull == false) return;

        Mng.canvas.SpawnItemAtMousePos(mResType, mResAmount, ItemLoc.ConvertResSlot, -1);
        UpdateResAmount(GameResType.Empty, new GameResAmount(0f, GameResUnit.Microgram));
    }

    private void MakeResTransparent()
    {
        kResImage.color = new Color(1, 1, 1, 0.5f);
        kResText.color = new Color(0, 0, 0, 0.5f);
    }

    private void MakeResOpaque()
    {
        kResImage.color = Color.white;
        kResText.color = Color.black;
    }

    public void UpdateImages()
    {
        kBackground.sprite = kBackgroundSprites[Mathf.Clamp((int)(mConvertProgress / mTotConvertTime * mConvertSpriteLength), 0, mConvertSpriteLength)];

        kPrevImage.sprite = Mng.canvas.GetResourceTypeIcon(mPrevType);
        kPrevText.text = Mng.canvas.GetAmountText(mPrevAmount);
        if(Mng.play.IsAmountZero(mPrevAmount)) kPrevText.text = "";

        if(mIsResFull == false)
        {
            MakeResTransparent();

            switch(mPrevType)
            {
                case GameResType.Nectar:
                    kResImage.sprite = Mng.canvas.GetResourceTypeIcon(GameResType.Honey);
                    kResText.text = Mng.canvas.GetAmountText(mPrevAmount);
                    break;
                case GameResType.Honey:
                    kResImage.sprite = Mng.canvas.GetResourceTypeIcon(GameResType.Wax);
                    kResText.text = Mng.canvas.GetAmountText(mPrevAmount);
                    break;
                default:
                    kResImage.sprite = Mng.canvas.GetResourceTypeIcon(GameResType.Empty);
                    kResText.text = "";
                    break;
            }

            if(Mng.play.IsAmountZero(mPrevAmount)) kResText.text = "";
        }
        else
        {   
            MakeResOpaque();

            kResImage.sprite = Mng.canvas.GetResourceTypeIcon(mResType);
            kResText.text = Mng.canvas.GetAmountText(mResAmount);
            if(Mng.play.IsAmountZero(mResAmount)) kResText.text = "";
        }
    }

    public GameResAmount AddPrevAmount(GameResType _type, GameResAmount _amount)
    {
        mPrevType = _type;

        GameResAmount sumAmount = Mng.play.AddResourceAmounts(_amount, mPrevAmount);
        GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

        GameResAmount maxAmount = GetMaxAmount(_type);

        if(Mng.play.CompareResourceAmounts(maxAmount, sumAmount)) //초과할때
        {   
            retAmount = Mng.play.SubtractResourceAmounts(sumAmount, maxAmount);
            UpdatePrevAmount(_type, maxAmount);
        }
        else
        {
            UpdatePrevAmount(_type, sumAmount);
        }

        return retAmount;
    }

    public void UpdatePrevAmount(GameResType _type, GameResAmount _amount)
    {
        mPrevType = _type;
        mPrevAmount = _amount;

        if(Mng.play.IsSameAmount(mPrevAmount, new GameResAmount(0f, GameResUnit.Microgram)))
        {
            mPrevType = GameResType.Empty;
        }

        mTotConvertTime = 10 * Mng.play.GetResourcePercent(mPrevAmount, GetMaxAmount(_type))/100;
        mConvertStepSec = mTotConvertTime / mConvertSpriteLength;

         if(Mng.play.IsAmountZero(mPrevAmount))
         {
            EndConvert();
         }
         else
         {
            StartConvert();
            print("start");
         }

        UpdateImages();
    }

    public GameResAmount AddResAmount(GameResType _type, GameResAmount _amount)
    {
        mResType = _type;

        GameResAmount sumAmount = Mng.play.AddResourceAmounts(_amount, mResAmount);
        GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

        GameResAmount maxAmount = GetMaxAmount(_type);

        if(Mng.play.CompareResourceAmounts(maxAmount, sumAmount)) //초과할때
        {   
            retAmount = Mng.play.SubtractResourceAmounts(sumAmount, maxAmount);
            UpdateResAmount(_type, maxAmount);
        }
        else
        {
            UpdateResAmount(_type, sumAmount);
        }

        return retAmount;
    }

    public void UpdateResAmount(GameResType _type, GameResAmount _amount)
    {
        mResType = _type;
        mResAmount = _amount;

        mIsResFull = Mng.play.IsAmountZero(mResAmount) == false;

        if(Mng.play.IsAmountZero(mResAmount))
         {
            StartConvert();
         }

        UpdateImages();
    }

    public bool IsPrevUsable(GameResType _type)
    {
        if(_type != mPrevType && mPrevType != GameResType.Empty) return false;
        return Mng.play.CompareResourceAmounts(GetMaxAmount(mPrevType) , mPrevAmount);
    }

    public GameResAmount GetMaxAmount(GameResType _type)
    {
        switch(_type)
        {
            case GameResType.Nectar:
                return Mng.play.kHive.mMaxItemAmounts[1];
            case GameResType.Pollen:
                return Mng.play.kHive.mMaxItemAmounts[2];
            case GameResType.Honey:
                return Mng.play.kHive.mMaxItemAmounts[0];
            case GameResType.Wax:
                return Mng.play.kHive.mMaxItemAmounts[3];
        }

        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    public void StartConvert()
    {
        if(mPrevType != GameResType.Nectar && mPrevType != GameResType.Honey)
            return;
        if(!(Mng.play.IsAmountZero(mResAmount)))
            return;
        
        if(mIsConverting == false)
        {
            StartCoroutine(ConvertCor());
            mIsConverting = true;
        }
    }

    public void EndConvert()
    {
        mIsConverting = false;
        mConvertProgress = 0;

        UpdateImages();
        StopAllCoroutines();
    }

    private IEnumerator ConvertCor()
    {
        mIsConverting = true;
        
        while(mConvertProgress < mTotConvertTime)
        {
            yield return new WaitForSeconds(mConvertStepSec);
            mConvertProgress += mConvertStepSec;

            UpdateImages();
        }

        GameResType newResType = GameResType.Empty;

        switch(mPrevType)
        {
            case GameResType.Honey:
                newResType = GameResType.Wax;
                break;
            case GameResType.Nectar:
                newResType = GameResType.Honey;
                break;
        }

        UpdateResAmount(newResType, mPrevAmount);
        UpdatePrevAmount(GameResType.Empty, new GameResAmount(0f, GameResUnit.Microgram));
        
        EndConvert();
    }
}
