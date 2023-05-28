using EnumDef;
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

        public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
        public float amount; //���� ���� ���� ��
        public GameResUnit unit; //���� ���� ����; ����� �� ������ ���� ������ش�
        public float maxAmount; //���� �� �ִ� �ִ� ��
        public GameResUnit maxUnit;

        /// <summary> Create : _ind 111 222 </summary>
        public Honeycomb(int _ind, Vector3 _pos, float _maxAmount, GameResUnit _maxUnit)
        {
            ind = _ind; //ind�� -1 �̸� �������� �ʴ´ٴ� ��
            pos = _pos;

            type = GameResType.Empty;
            amount = 0f;
            unit = GameResUnit.Microgram;
            maxAmount = _maxAmount;
            maxUnit = _maxUnit;
        }

        public bool IsFull() //�� ���� �� ���ִ��� Ȯ��
        {
            if (unit != maxUnit)
            {
                return (int)unit < (int)maxUnit;
            }
            else
            {
                return (int)amount < (int)maxAmount;
            }
        }
    }
}
