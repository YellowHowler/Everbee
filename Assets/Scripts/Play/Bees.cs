using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bees : MonoBehaviour
{
    public GameObject kBeeObj;

    public void CreateBee(Vector3 _pos)
    {
        GameObject newBee = Instantiate(kBeeObj, _pos, Quaternion.identity);
        newBee.transform.parent = transform;
        Mng.canvas.kJob.AddBeeJobUI(newBee.GetComponent<Bee>());
    }

    IEnumerator Start()
    {
        while (PlayManager.Instance.kHive == null)
            yield return null;
        while (PlayManager.Instance.kGarden == null)
            yield return null;

        yield return new WaitForSeconds(1);

        CreateBee(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
