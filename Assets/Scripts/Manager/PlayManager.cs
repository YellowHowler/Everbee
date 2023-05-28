using ClassDef;
using EnumDef;
using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;

    public Hive mHive;
    public Garden mGarden;

    void Awake()
    {
        Instance = this;

        mHive = GameObject.Find("Stage/Hive").GetComponent<Hive>();
        mGarden = GameObject.Find("Stage/Garden").GetComponent<Garden>();

        EasyTouch.On_TouchStart += OnTouch;
    }

    public void GameStart()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    private void OnTouch(Gesture gesture)
    {
        if (gesture.pickedObject == null)
            return;

        //Mng.canvas.kResource.SetText("??");

        /*
        if( kHive.kQueenBee.gameObject == gesture.pickedObject )
        {
            PlayerCamera.Instance.SetFollow(kHive.kQueenBee.transform);
        }
        */
        
        Debug.Log(gesture.pickedObject.name);
    }
}

