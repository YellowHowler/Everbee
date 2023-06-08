using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTogglePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuildMenuBtnClick()
    {
        GameObject buildMenu = Mng.canvas.kBuild.gameObject;

        buildMenu.SetActive(true);
    }

    public void OnJobMenuBtnClick()
    {
        GameObject jobMenu = Mng.canvas.kJob.gameObject;

        jobMenu.SetActive(true);
    }
}
