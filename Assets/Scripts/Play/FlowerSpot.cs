using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpot : MonoBehaviour
{
    public int ind;

    public bool isTarget = false;
    public Vector3 pos { get { return transform.position; } set { pos = value; } }

    public FlowerType type;

    public bool occupied;

    public GameResAmount nectarAmount;
    public GameResAmount pollenAmount;
}
