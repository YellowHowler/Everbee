using System.Collections;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitlePanel: MonoBehaviour
{
	public Button StartButton;
	public Button ContinueButton;
	public Button OptionsButton;
	public Button QuitButton;
	public LHS.CLHSDialogUI kDialoguePopup;

	// Use this for initialization
	void Awake()
	{
		StartButton.onClick.AddListener(OnStartClicked);
		ContinueButton.onClick.AddListener(OnContinueClicked);
		QuitButton.onClick.AddListener(OnQuitClicked);

		kDialoguePopup.Init();

		ContinueButton.interactable = SaveManager.Instance.IsThereSaveData();
	}

	private void StartGame()
	{
		PlayManager.MustLoadSaveData = false;
		SceneManager.LoadScene("Scenes/Everbee");
	}
	private void OnStartClicked()
	{
		if (SaveManager.Instance.IsThereSaveData())
		{
			kDialoguePopup.Show("A fresh start may erase existing saves.\r\n\r\nAre you sure you want to start fresh?", LHS.CLHSDialogUI.EButtonType.YESNO, (result) =>
			{
				if (result == LHS.CLHSDialogUI.EDialogResult.YES)
					StartGame();

				return true;
			});
		}
		else
			StartGame();
	}

	private void OnContinueClicked()
	{
		PlayManager.MustLoadSaveData = true;
		SceneManager.LoadScene("Scenes/Everbee");
	}

	private void OnQuitClicked()
	{
		UnityEngine.Application.Quit();
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#endif
	}
}
