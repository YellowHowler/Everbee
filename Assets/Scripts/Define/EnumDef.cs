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
        Cosmos = 1,
        OxeyeDaisy,
        Lavender,
        CaliforniaPoppy
    }

    public enum Job
    {
        Idle = 0,
        Collect = 1, //꽃에서 자원 수집
        Build = 2,
    }

    public enum StructureType
    {
        None = 0,
        Storage,
        Dry,
        Coalgulate,
        Hatchtery,
    }

    public enum HoneycombDirection
    {
        None = 0,
        TopLeft,
        TopRight,
        Left,
        Right,
        BottomLeft,
        BottomRight,
    }
}
