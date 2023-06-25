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

    [HideInInspector] public GameResAmount[] mResAmounts;

    void Awake()
    {
        mAnimator = GetComponentInChildren<Animator>();

        mResAmounts = new GameResAmount[4];
        for(int i = 0; i < mResAmounts.Length; i++)
        {
            mResAmounts[i] = new GameResAmount(0, GameResUnit.Microgram);
        }
    }

    void Update()
    {
        
    }

    public void AddResource(GameResType _type, GameResAmount _amount)
    {
        mResAmounts[(int)_type] = Mng.play.AddResourceAmounts(mResAmounts[(int)_type], _amount);
    }

    private void OnMouseDown()
    {
        print("hi");
        Mng.canvas.kQueen.gameObject.SetActive(true);
    }
}
