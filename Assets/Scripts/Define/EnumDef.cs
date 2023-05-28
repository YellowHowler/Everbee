using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumDef
{
    public enum GameResUnit
    {
        Microgram = 1,
        Milligram = 2,
        Gram = 3,
        Kilogram = 4
    }

    public enum GameResType
    {
        Empty = -1,
        Honey = 0,
        Nectar = 1,
        Pollen = 2,
        Wax = 3,
        Egg = 4,
        Larvae = 5
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
