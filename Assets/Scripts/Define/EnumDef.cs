using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumDef
{
    public enum Unit
    {
        microgram = 1,
        milligram = 2,
        gram = 3,
        kilogram = 4
    }

    public enum Resource
    {
        empty = -1,
        honey = 0,
        nectar = 1,
        pollen = 2,
        wax = 3,
        egg = 4,
        larvae = 5
    }

    public enum FlowerType
    {
        cosmos = 1,
        oxeye_daisy,
        lavender,
        california_poppy
    }

    public enum Job
    {
        idle = 0,
        collect = 1, //꽃에서 자원 수집
    }
}
