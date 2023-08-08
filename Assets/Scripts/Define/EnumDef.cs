using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumDef
{
    [Serializable]
    public enum GameResUnit
    {
        Microgram = 1,
        Milligram = 2,
        Gram = 3,
        Kilogram = 4
    }

    [Serializable]
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

    [Serializable]
    public enum BeeStage
    {
        Egg = 0,
        Larvae = 1,
        Pupa = 2,
        Bee = 3,
    }

    [Serializable]
    public enum BeeThinking
    {
        None=0,
        
        MovingToFlower,             // 꽃으로 이동중
        MovingToStorage,            // Storage 로 이동중
        MovingToLarvae,             // Larvae 로 이동중
        MovingToBuild, 

        CollectingFromFlower,
        Feeding,
        Building,
        
        NoAvailableFlower = 100,    // 현재 캘 수 있는 꽃이 없다
        NoAvailableNectarStorage,   // 현재 이용할 수 있는 Nectar 창고가 없다
        NoAvailablePollenStorage,   // 현재 이용할 수 있는 Pollen 창고가 없다
        NoAvailableWaxStorage,      // 현재 이용할 수 있는 Wax 창고가 없다
        NoAvailableHoneyStorage,    // 현재 이용할 수 있는 Honey 창고가 없다
        NoAvailableLarvae,          // 현재 내가 Feeding 할만한 Larvae 가 없다
        NoBuildingStructure,        // 현재 Build 예정인 Structure 가 없다

        NoPollenInThisStorage,      // 현재 이 comb 에는 Pollen 이 없다
        NoHoneyInThisStorage,       // 현재 이 comb 에는 Honey 가 없다
        NoWaxInThisStorage,         // 현재 이 comb 에는 Wax 가 없다
    }

    [Serializable]
    public enum Job
    {
        Idle = 0,
        Collect = 1, //꽃에서 자원 수집
        Build = 2,
        Feed = 3, 
    }

    [Serializable]
    public enum CollectState
    {
        GoToFlower = 0,
        CollectResource,
        GoToPollen,
        StorePollen,
        GoToNectar,
        StoreNectar,
    }

    [Serializable]
    public enum QueenState
    {
        Wander = 0,
        WaitForTarget = 1,
        GoToTarget = 2,
        LayEgg = 3,
    }

    [Serializable]
    public enum StructureType
    {
        Building = -1,
        None = 0,
        Storage = 1,
        Dryer = 2,
        Coalgulate = 3,
        Hatchtery = 4,
    }

    [Serializable]
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
