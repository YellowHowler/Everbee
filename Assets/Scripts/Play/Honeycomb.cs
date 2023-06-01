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

    public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResAmount maxAmount;

    public bool IsFull() //�� ���� �� ���ִ��� Ȯ��
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

    /// <summary> �Ķ���� ����ŭ ����, ��������� �����Ѵٸ� �����Ѹ�ŭ ��ȯ </summary>
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

        if (PlayManager.Instance.CompareResourceAmounts(retAmount, maxAmount) == true) //������� �Ⱥ����ϸ� �״�� ����
        {
            amount = retAmount;
            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        retAmount = PlayManager.Instance.SubtractResourceAmounts(retAmount, maxAmount);
        amount = maxAmount;

        return retAmount;
    }
}