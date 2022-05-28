using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class BotDataHandler
{
	public static BotSaveData ReadBotDataFromJson(string dataPath)
	{
		string json = "";
		//string jsonPath = Application.persistentDataPath + "/SavedBotData.json";
		string jsonPath = dataPath;
		BotSaveData savedBotData;

		if (File.Exists(jsonPath))
		{
			json = File.ReadAllText(jsonPath);
			savedBotData = JsonUtility.FromJson<BotSaveData>(json);
			Debug.Log(json);
		}
		else
		{
			Debug.LogWarning($"No Json file found in {jsonPath}");
			File.Create(jsonPath);
			return null;
		}

		return savedBotData;

	}
	public static void SaveBotDataToJson(string dataPath, BotSaveData data)
	{
		if (!File.Exists(dataPath))
			File.Create(dataPath);

		string json = JsonUtility.ToJson(data);

		File.WriteAllText(dataPath, json);
	}



}
