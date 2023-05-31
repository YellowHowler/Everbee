using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassDef
{
    public class Honeycomb
    {
        public int ind; //�迭���� ���°����
        public Vector3 pos;

        public bool isTarget;

        public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
        public GameResAmount amount;
        public GameResAmount maxAmount;

        /// <summary> Create : _ind 111 222 </summary>
        public Honeycomb(int _ind, Vector3 _pos, GameResAmount _maxAmount)
        {
            ind = _ind; //ind�� -1 �̸� �������� �ʴ´ٴ� ��
            pos = _pos;

            isTarget = false;

            type = GameResType.Empty;
            amount = new GameResAmount(0f, GameResUnit.Microgram);
            maxAmount = _maxAmount;
        }

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
    }
}
