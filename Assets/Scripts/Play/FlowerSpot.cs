using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpot : MonoBehaviour
{
    public int ind;
    public Vector3 pos;

    public FlowerType type;

    public bool occupied;

    public float nectarAmount;
    public GameResUnit nectarUnit;
    public float pollenAmount;
    public GameResUnit pollenUnit;

    public FlowerSpot(int _ind, Vector3 _pos, FlowerType _type)
    {
        ind = _ind;
        pos = _pos;

        type = _type;

        occupied = false;

        nectarAmount = 1f;
        nectarUnit = GameResUnit.Microgram;
        pollenAmount = 1f;
        pollenUnit = GameResUnit.Microgram;
    }
}
