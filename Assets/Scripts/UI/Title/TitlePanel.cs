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

	// Use this for initialization
	void Awake()
	{
		StartButton.onClick.AddListener(OnStartClicked);
		ContinueButton.onClick.AddListener(OnContinueClicked);
		QuitButton.onClick.AddListener(OnQuitClicked);
	}

	private void OnStartClicked()
	{
		PlayManager.MustLoadSaveData = false;
		SceneManager.LoadScene("Scenes/Everbee");
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
