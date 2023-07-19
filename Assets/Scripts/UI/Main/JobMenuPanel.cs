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
        newJobManageObj.GetComponent<BeeJobManage>().SetBee(_bee);

        var rect = kJobScrollContents.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, kJobScrollContents.childCount * newJobManageObj.GetComponent<RectTransform>().sizeDelta.y);
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
