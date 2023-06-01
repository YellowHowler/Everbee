using ClassDef;
using EnumDef;
using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StructDef;
using static UnityEngine.UI.CanvasScaler;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;

    public Hive kHive;
    public Garden kGarden;

    public GameResAmount mHoneycombMaxAmount = new GameResAmount(3f, GameResUnit.Microgram);

    void Awake()
    {
        Instance = this;

        kHive = GameObject.Find("Stage/Hive").GetComponent<Hive>();
        kGarden = GameObject.Find("Stage/Garden").GetComponent<Garden>();

        EasyTouch.On_TouchStart += OnTouch;
    }

    public void GameStart()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    private void OnTouch(Gesture gesture)
    {
        if (gesture.pickedObject == null)
            return;

        //Mng.canvas.kResource.SetText("??");

        /*
        if( kHive.kQueenBee.gameObject == gesture.pickedObject )
        {
            PlayerCamera.Instance.SetFollow(kHive.kQueenBee.transform);
        }
        */
        
        Debug.Log(gesture.pickedObject.name);
    }

    public GameResAmount AddResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        int aUnit = (int)_resAmountA.unit;
        int bUnit = (int)_resAmountB.unit;

        if (Mathf.Abs(aUnit - bUnit) >= 2)
        {
            if(aUnit > bUnit) 
            {
                return _resAmountA;
            }
            else
            {
                return _resAmountB;
            }
        }

        float retAmount = 0;
        GameResUnit retUnit = GameResUnit.Microgram;

        if (aUnit == bUnit)
        {
            retAmount = _resAmountA.amount + _resAmountB.amount;
            retUnit = _resAmountA.unit;
        }
        else
        {
            if(aUnit > bUnit)
            {
                retAmount = _resAmountA.amount + _resAmountB.amount * 0.001f;
                retUnit = _resAmountA.unit;
            }
            else
            {
                retAmount = _resAmountB.amount + _resAmountA.amount * 0.001f;
                retUnit = _resAmountB.unit;
            }
        }

        return UpdateUnit(new GameResAmount(retAmount, retUnit));
    }

    public GameResAmount SubtractResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        _resAmountB.amount = -1 * _resAmountB.amount;
        return AddResourceAmounts(_resAmountA, _resAmountB);
    }

    public GameResAmount UpdateUnit(GameResAmount _amount)
    {
        if(_amount.amount >= 1000f)
        {
            _amount.amount /= 1000;
            _amount.unit = (GameResUnit)((int)_amount.unit + 1);
        }
        else if (_amount.amount < 1)
        {
            if (_amount.unit == GameResUnit.Microgram)
            {
                return new GameResAmount(0f, GameResUnit.Microgram);
            }
            _amount.amount *= 1000;
            _amount.unit = (GameResUnit)((int)_amount.unit - 1);
        }

        return _amount;
    }

    /// <summary> 첫번째가 더 크면 false, 두번째가 더 크면 true </summary>
    public bool CompareResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        if((int)_resAmountA.unit != (int) _resAmountB.unit)
        {
            return (int)_resAmountA.unit < (int)_resAmountB.unit;
        }

        return _resAmountA.amount < _resAmountB.amount;
    }
}

