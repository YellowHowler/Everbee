using ClassDef;
using EnumDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static public Manager Instance { get; private set; }

    public Transform kStage;

    [Header("플레이 매니저")]
    public PlayManager kPlayManager;
    [Header("사운드 매니저")]
    public SoundManager kSoundManager;
    [Header("벌 매니저")]
    public Bees kBees;
    [Header("벌집 매니저")]
    public Hive kHive;
    [Header("정원 매니저")]
    public Garden kGarden;
	[Header("인벤토리 매니저")]
	public Inventory kInventory;


	private void Awake()
    {
        Instance = this;
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	IEnumerator Start()
    {
        GameObject go = Instantiate(kSoundManager.gameObject);
        go.transform.parent = transform;
        go.name = "SoundManager";

        while (SoundManager.Instance == null)
            yield return null;

        while (MainCanvas.Instance == null)
            yield return null;

        while (PlayerCamera.Instance == null)
            yield return null;

        go = Instantiate(kGarden.gameObject);
        go.transform.parent = kStage;
        go.name = "Garden";

        while (Garden.Instance == null)
            yield return null;

        go = Instantiate(kHive.gameObject);
        go.transform.parent = kStage;
        go.name = "Hive";

        while (Hive.Instance == null)
            yield return null;

		go = Instantiate(kInventory.gameObject);
		go.transform.parent = kStage;
		go.name = "Inventory";

		go = Instantiate(kPlayManager.gameObject);
        go.transform.parent = transform;
        go.name = "PlayManager";

        while (PlayManager.Instance == null)
            yield return null;

        go = Instantiate(kBees.gameObject);
        go.transform.parent = kStage;
        go.name = "Bees";
	}
}
