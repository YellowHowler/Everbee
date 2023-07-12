using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveManager: MonoBehaviour
{
	static public SaveManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				GameObject gobj = new GameObject("SaveManager");
				_Instance = gobj.AddComponent<SaveManager>();
				GameObject.DontDestroyOnLoad(gobj);
			}

			return _Instance;
		}
	}
	static private SaveManager _Instance;

	public Hive.CSaveData HiveSaveData = new Hive.CSaveData();
	// Garden 은 구조를 바꿔야 한다.

	private string FileName = "Save.dat";

	public bool Save()
	{
		PlayManager.Instance.kHive.ExportTo(HiveSaveData);

		string json = JsonUtility.ToJson(this);

		string path = string.Format("{0}\\{1}", Application.persistentDataPath, FileName);
		File.WriteAllText(path, json);

		return true;
	}

	public bool Load()
	{
		string path = string.Format("{0}\\{1}", Application.persistentDataPath, FileName);
		if (!File.Exists(path))
			return false;

		try
		{
			string json = File.ReadAllText(path);
			JsonUtility.FromJsonOverwrite(json, this);

			PlayManager.Instance.kHive.ImportFrom(HiveSaveData);
		}
		catch (System.Exception ex)
		{
			Debug.LogError(string.Format("Load failed: {0}", ex));
			return false;
		}

		return true;
	}
}
