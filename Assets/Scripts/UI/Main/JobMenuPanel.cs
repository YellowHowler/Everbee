using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobMenuPanel : PopupBase
{
    public Transform kJobScrollContents;
    public GameObject kBeeJobManageObj;

	override public void ProcessEscapeKey()
	{
		OnJobMenuBgClick();
		base.ProcessEscapeKey();
	}

	public void AddBeeJobUI(Bee _bee)
    {
        GameObject newJobManageObj = Instantiate(kBeeJobManageObj, kJobScrollContents);
        newJobManageObj.GetComponent<BeeJobManage>().kBee = _bee;
    }

    public void OnJobMenuBgClick()
    {
        Mng.canvas.HideMenu();
        Hide();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
