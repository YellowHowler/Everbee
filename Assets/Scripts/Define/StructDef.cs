using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;

namespace StructDef
{
    public struct GameResAmount
    {
        public float amount;
        public GameResUnit unit;

        public GameResAmount(float _amount, GameResUnit _unit)
        {
            amount = _amount;
            unit = _unit;
        }
    }

    public struct VectorBound
    {
        public float start;
        public float end;

        public VectorBound(float _start, float _end)
        {
            start = _start;
            end = _end;
        }
    }
}
