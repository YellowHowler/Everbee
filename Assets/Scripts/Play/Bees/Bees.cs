using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bees : MonoBehaviour
{
    public GameObject kBeeObj;
    public GameObject kQueenBeeObj;

    private List<Bee> mBeeList = new List<Bee>();

    public void CreateBee(Vector3 _pos)
    {
        GameObject newBee = Instantiate(kBeeObj, _pos, Quaternion.identity);
        newBee.transform.parent = transform;

        Bee bee = newBee.GetComponent<Bee>();

        mBeeList.Add(bee);
        Mng.canvas.kJob.AddBeeJobUI(bee);
    }

    public void CreateBee(Vector3 _pos, int _level)
    {
        GameObject newBee = Instantiate(kBeeObj, _pos, Quaternion.identity);
        newBee.transform.parent = transform;
<<<<<<< HEAD
        newBee.GetComponent<Bee>().UpdateLevel(_level);
        Mng.canvas.kJob.AddBeeJobUI(newBee.GetComponent<Bee>());
=======

        Bee bee = newBee.GetComponent<Bee>();

        mBeeList.Add(bee);
        bee.UpdateLevel(_level);
        bee.UpdateStage(_stage);
        Mng.canvas.kJob.AddBeeJobUI(bee);
>>>>>>> parent of 9ba245c (Revert "-")
    }

    public void CreateQueenBee(Vector3 _pos)
    {
        GameObject newQueenBee = Instantiate(kQueenBeeObj, _pos, Quaternion.identity);
        newQueenBee.transform.parent = transform;
    }

<<<<<<< HEAD
=======
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

>>>>>>> parent of 9ba245c (Revert "-")
    IEnumerator Start()  
    {
        while (PlayManager.Instance.kHive == null)
            yield return null;
        while (PlayManager.Instance.kGarden == null)
            yield return null;

        yield return new WaitForSeconds(1);

        for(int i = 0; i < 4; i++)
        {
            CreateBee(Vector3.zero);
        }

        CreateQueenBee(new Vector3(0, 15, 0));
    }  

    void Update()
    {
        
    }
}
