using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTogglePanel : MonoBehaviour
{
    public Button kJobBtn;
    public Button kBuildBtn;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuildMenuBtnClick()
    {
        kBuildBtn.interactable = false;

        Mng.canvas.ShowMenu();

        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Show();
        Mng.canvas.kJob.Hide();
	}

	public void OnJobMenuBtnClick()
    {
        kJobBtn.interactable = false;

        Mng.canvas.ShowMenu();
        
        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Hide();
        Mng.canvas.kJob.Show();
    }
}
