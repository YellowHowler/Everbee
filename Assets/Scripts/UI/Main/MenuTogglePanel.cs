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
        Mng.canvas.ShowMenu();

        GameObject buildMenu = Mng.canvas.kBuild.gameObject;
        GameObject jobMenu = Mng.canvas.kJob.gameObject;

        Mng.canvas.kQueen.gameObject.SetActive(false);
        jobMenu.SetActive(false);
        buildMenu.SetActive(true);
    }

    public void OnJobMenuBtnClick()
    {
        Mng.canvas.ShowMenu();
        
        GameObject buildMenu = Mng.canvas.kBuild.gameObject;
        GameObject jobMenu = Mng.canvas.kJob.gameObject;

        Mng.canvas.kQueen.gameObject.SetActive(false);
        buildMenu.SetActive(false);
        jobMenu.SetActive(true);
    }
}
