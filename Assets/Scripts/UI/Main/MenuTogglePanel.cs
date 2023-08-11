using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTogglePanel : MonoBehaviour
{
    public Button kJobBtn;
    public Button kBuildBtn;
    public Button kQueenBtn;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuildMenuBtnClick()
    {
        Mng.play.kCamera.StopFollow();

        kBuildBtn.interactable = false;

        Mng.canvas.ShowMenu();

        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Show();
        Mng.canvas.kJob.Hide();
	}

	public void OnJobMenuBtnClick()
    {
        Mng.play.kCamera.StopFollow();

        kJobBtn.interactable = false;

        Mng.canvas.ShowMenu();
        
        Mng.canvas.kQueen.Hide();
        Mng.canvas.kBuild.Hide();
        Mng.canvas.kJob.Show();
    }

    public void OnQueenMenuBtnClick()
    {
        kQueenBtn.interactable = false;
        
        Mng.canvas.kQueen.Show();
        Mng.canvas.kBuild.Hide();
        Mng.canvas.kJob.Hide();

        QueenBee targetQueen = Mng.play.kHive.mActiveQueenBee;

        Mng.canvas.kQueen.mTargetQueen = targetQueen;
        Mng.canvas.kQueen.UpdateSliders(targetQueen.mCurHoney, targetQueen.mCurPollen);

        Mng.play.kCamera.SetFollow(targetQueen.gameObject.transform);
    }
}
