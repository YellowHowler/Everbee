using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobMenuPanel : MonoBehaviour
{
    public Transform kJobScrollContents;
    public GameObject kBeeJobManageObj;

    public void AddBeeJobUI(Bee _bee)
    {
        GameObject newJobManageObj = Instantiate(kBeeJobManageObj, kJobScrollContents);
        newJobManageObj.GetComponent<BeeJobManage>().kBee = _bee;
    }

    public void OnJobMenuBgClick()
    {
        Mng.canvas.kIsViewingMenu = false;
        gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
