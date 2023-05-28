using EnumDef;
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

        public GameResType type; //무슨 자원을 저장하고 있는지
        public float amount; //단위 제외 저장 양
        public GameResUnit unit; //저장 양의 단의; 계산할 때 단위를 맞춰 계산해준다
        public float maxAmount; //담을 수 있는 최대 양
        public GameResUnit maxUnit;

        /// <summary> Create : _ind 111 222 </summary>
        public Honeycomb(int _ind, Vector3 _pos, float _maxAmount, GameResUnit _maxUnit)
        {
            ind = _ind; //ind가 -1 이면 존재하지 않는다는 뜻
            pos = _pos;

            type = GameResType.Empty;
            amount = 0f;
            unit = GameResUnit.Microgram;
            maxAmount = _maxAmount;
            maxUnit = _maxUnit;
        }

        public bool IsFull() //그 셀이 꽉 차있는지 확인
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
