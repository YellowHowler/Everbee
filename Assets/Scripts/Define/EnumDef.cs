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

    public enum BeeStage
    {
        Egg = 0,
        Larvae = 1,
        Pupa = 2,
        Bee = 3,
    }

    public enum Job
    {
        Idle = 0,
        Collect = 1, //꽃에서 자원 수집
        Build = 2,
        Feed = 3, 
    }

    public enum CollectState
    {
        GoToFlower = 0,
        CollectResource,
        GoToPollen,
        StorePollen,
        GoToNectar,
        StoreNectar,
    }

    public enum QueenState
    {
        Wander = 0,
        WaitForTarget = 1,
        GoToTarget = 2,
        LayEgg = 3,
    }

    //버튼은 숫자로 함수 부르는걸로 되어있어 숫자 바꿀 때 주의
    public enum StructureType
    {
        None = 0,
        Building = 1,
        Storage = 2,
        Dryer = 3,
        Coalgulate = 4,
        Hatchtery = 5,
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
