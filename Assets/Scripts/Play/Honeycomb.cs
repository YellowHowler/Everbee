using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{
    public SpriteRenderer kSprieRenderer;

    public Vector3 pos { get { return transform.position; } set { pos = value; } }

    public bool isTarget;

    public GameResType type; //무슨 자원을 저장하고 있는지
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResAmount maxAmount;

    public bool IsFull() //그 셀이 꽉 차있는지 확인
    {
        if (amount.unit != maxAmount.unit)
        {
            return (int)amount.unit < (int)maxAmount.unit;
        }
        else
        {
            return (int)amount.amount < (int)maxAmount.amount;
        }
    }

    /// <summary> 파라미터 값만큼 저장, 저장공간이 부족한다면 부족한만큼 반환 </summary>
    public GameResAmount StoreResource(GameResType _type, GameResAmount _amount)
    {
        if (type == GameResType.Empty || type == _type)
        {
            type = _type;
        }
        else
        {
            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        GameResAmount retAmount = PlayManager.Instance.AddResourceAmounts(_amount, amount);

        if (PlayManager.Instance.CompareResourceAmounts(retAmount, maxAmount) == true) //저장공간 안부족하면 그대로 저장
        {
            amount = retAmount;
            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        retAmount = PlayManager.Instance.SubtractResourceAmounts(retAmount, maxAmount);
        amount = maxAmount;

        return retAmount;
    }
}