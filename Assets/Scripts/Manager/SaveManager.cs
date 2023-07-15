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
	public Garden.CSaveData GardenSaveData = new Garden.CSaveData();

	private string FileName = "Save.dat";
	private string GetFullPath()
	{
		return string.Format("{0}\\{1}", Application.persistentDataPath, FileName);
	}

	public bool IsThereSaveData()
	{
		string path = GetFullPath();
		return File.Exists(path);
	}

	public bool Save()
	{
		PlayManager.Instance.kHive.ExportTo(HiveSaveData);
		PlayManager.Instance.kGarden.ExportTo(GardenSaveData);

		string json = JsonUtility.ToJson(this);

		string path = GetFullPath();
		File.WriteAllText(path, json);

		return true;
	}

	public bool Load()
	{
		string path = GetFullPath();
		if (!File.Exists(path))
			return false;

		try
		{
			string json = File.ReadAllText(path);
			JsonUtility.FromJsonOverwrite(json, this);

			PlayManager.Instance.kHive.ImportFrom(HiveSaveData);
			PlayManager.Instance.kGarden.ImportFrom(GardenSaveData);
		}
		catch (System.Exception ex)
		{
			Debug.LogError(string.Format("Load failed: {0}", ex));
			return false;
		}

		return true;
	}
}
