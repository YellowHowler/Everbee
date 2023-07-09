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

        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Show();
        Mng.canvas.kJob.Hide();
	}

	public void OnJobMenuBtnClick()
    {
        Mng.canvas.ShowMenu();
        
        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Hide();
        Mng.canvas.kJob.Show();
    }
}
