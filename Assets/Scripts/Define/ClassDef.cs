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
        public int ind; //배열에서 몇번째인지
        public Vector3 pos;

        public bool isTarget;

        public GameResType type; //무슨 자원을 저장하고 있는지
        public GameResAmount amount;
        public GameResAmount maxAmount;

        /// <summary> Create : _ind 111 222 </summary>
        public Honeycomb(int _ind, Vector3 _pos, GameResAmount _maxAmount)
        {
            ind = _ind; //ind가 -1 이면 존재하지 않는다는 뜻
            pos = _pos;

            isTarget = false;

            type = GameResType.Empty;
            amount = new GameResAmount(0f, GameResUnit.Microgram);
            maxAmount = _maxAmount;
        }

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
    }
}
