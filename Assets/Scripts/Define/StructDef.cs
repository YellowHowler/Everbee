using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;

namespace StructDef
{
    public struct Honeycomb 
    {
        public int ind; //배열에서 몇번째인지
        public Vector3 pos;

        public Resource type; //무슨 자원을 저장하고 있는지
        public float amount; //단위 제외 저장 양
        public Unit unit; //저장 양의 단의; 계산할 때 단위를 맞춰 계산해준다
        public float maxAmount; //담을 수 있는 최대 양
        public Unit maxUnit;

        public Honeycomb(int _ind, Vector3 _pos, float _maxAmount, Unit _maxUnit)
        {
            ind = _ind; //ind가 -1 이면 존재하지 않는다는 뜻
            pos = _pos;

            type = Resource.empty;
            amount = 0f;
            unit = Unit.microgram;
            maxAmount = _maxAmount;
            maxUnit = _maxUnit;
        }
    }

    public struct FlowerSpot
    {
        public int ind;
        public Vector3 pos;

        public FlowerType type;

        public bool occupied;

        public float nectarAmount;
        public Unit nectarUnit;
        public float pollenAmount;
        public Unit pollenUnit;

        public FlowerSpot(int _ind, Vector3 _pos, FlowerType _type)
        {
            ind = _ind;
            pos = _pos;

            type = _type;

            occupied = false;

            nectarAmount = 1f;
            nectarUnit = Unit.microgram;
            pollenAmount = 1f;
            pollenUnit = Unit.microgram;
        }
    }
}
