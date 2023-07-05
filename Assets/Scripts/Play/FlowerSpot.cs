using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpot : MonoBehaviour
{
    [HideInInspector] public Garden mGarden;

    [HideInInspector] public bool isTarget = false;
    public Vector3 pos;

    public FlowerType type;

    public float nectar;
    public GameResUnit nectarUnit;
    public float pollen;
    public GameResUnit pollenUnit;

    public GameResAmount nectarAmount;
    public GameResAmount pollenAmount;

    private void Awake()
    {
        nectarAmount = new GameResAmount(nectar, nectarUnit);
        pollenAmount = new GameResAmount(pollen, pollenUnit);

        pos = transform.position;
    }
}
