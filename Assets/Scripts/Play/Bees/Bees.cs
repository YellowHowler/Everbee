using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Bees : MonoBehaviour
{
    public GameObject kBeeObj;
    public GameObject kQueenBeeObj;

    private List<Bee> mBeeList = new List<Bee>();
    private QueenBee mQueenBee;
    

    public Bee CreateBee(Vector3 _pos)
    {
        GameObject newBee = Instantiate(kBeeObj, _pos, Quaternion.identity);
        newBee.transform.parent = transform;

        Bee bee = newBee.GetComponent<Bee>();

        mBeeList.Add(bee);
        Mng.canvas.kJob.AddBeeJobUI(bee);

        return bee;
    }

    public Bee CreateBee(Vector3 _pos, int _level, BeeStage _stage)
    {
        GameObject newBee = Instantiate(kBeeObj, _pos, Quaternion.identity);
        newBee.transform.parent = transform;

        Bee bee = newBee.GetComponent<Bee>();

        mBeeList.Add(bee);
        bee.UpdateLevel(_level);
        bee.UpdateStage(_stage);
        Mng.canvas.kJob.AddBeeJobUI(bee);

        return bee;
    }

    public QueenBee CreateQueenBee(Vector3 _pos)
    {
        if (mQueenBee == null)
            mQueenBee = Instantiate(kQueenBeeObj, _pos, Quaternion.identity).GetComponent<QueenBee>();
        mQueenBee.pos = _pos;

        return mQueenBee;
    }

    public Bee FindLarvae()
    {
        foreach(Bee b in mBeeList)
        {
            if(b.mCurStage == BeeStage.Larvae && b.mIsTarget == false)
            {
                return b;
            }
        }

        return null;
    }

    private void Awake()
    {
        Mng.play.kBees = this;
    }

    public void InitDefault()  
    {
        for(int i = 0; i < 4; i++)
        {
            CreateBee(Vector3.zero, 8, BeeStage.Bee);
        }

        CreateQueenBee(new Vector3(0, 15, 0));
    }  

    void Update()
    {
        
    }


	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
        public List<Bee.CSaveData> mBeeList = new List<Bee.CSaveData>();
        public QueenBee.CSaveData mQueenBee = new QueenBee.CSaveData();
	}

	public void ExportTo(CSaveData savedata)
	{
        savedata.mBeeList.Clear();

        foreach(var bee in mBeeList)
        {
            var beesave = new Bee.CSaveData();
            bee.ExportTo(beesave);
            savedata.mBeeList.Add(beesave);
        }

        if (mQueenBee == null)
            savedata.mQueenBee = null;
        else
            mQueenBee.ExportTo(savedata.mQueenBee);
	}

	public void ImportFrom(CSaveData savedata)
	{
        foreach(var bee in mBeeList)
            GameObject.Destroy(bee.gameObject);
        mBeeList.Clear();

        foreach(var beesave in savedata.mBeeList)
        {
            var bee = CreateBee(beesave.Pos);
            bee.ImportFrom(beesave);
        }

        if (savedata.mQueenBee == null)
        {
            // 그래도 한마리는 만들어 주어야 함.
            CreateQueenBee(new Vector3(0, 15, 0));
        }
        else
        {
            var queenbee = CreateQueenBee(savedata.mQueenBee.Pos);
            queenbee.ImportFrom(savedata.mQueenBee);
        }
	}
}
