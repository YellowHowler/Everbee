using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class QueenBee : MonoBehaviour
{
    private Animator mAnimator;

    [HideInInspector] public GameResAmount mCurHoney = new GameResAmount(0f, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurPollen = new GameResAmount(0f, GameResUnit.Microgram);

    void Awake()
    {
        mAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);
    }

    void Update()
    {
        
    }

    public void AddResource(GameResType _type, GameResAmount _amount)
    {
        switch(_type)
        {
            case GameResType.Honey:
                mCurHoney = Mng.play.AddResourceAmounts(mCurHoney, _amount);
                break;

            case GameResType.Pollen:
                mCurPollen = Mng.play.AddResourceAmounts(mCurPollen, _amount);
                break;
        }
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);
    }

    private void OnMouseDown()
    {
        print("hi");
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);
        Mng.canvas.kQueen.gameObject.SetActive(true);
    }

    public void LayEgg()
    {
        
    }
}
