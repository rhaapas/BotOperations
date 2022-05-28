using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manager for saving bot data, Does not work at the moment. 
/// TODO: Fix Saving and loading
/// </summary>
public class SaveDataManager : MonoBehaviour
{
	[SerializeField] private string botSaveDataFileName;
	[SerializeField] private BotSaveData botSaveData = new BotSaveData();

	private string botSaveDataPath;

	public bool SaveDataRetrieved { get { return saveDataRetrieved; } }
	[SerializeField] private bool saveDataRetrieved;

	private void Awake()
	{
		botSaveDataPath = Path.Combine(Application.persistentDataPath, botSaveDataFileName);
		botSaveData = BotDataHandler.ReadBotDataFromJson(botSaveDataPath);
		saveDataRetrieved = true;
	}
	public void SaveBotData()
	{
		BotDataHandler.SaveBotDataToJson(botSaveDataPath, botSaveData);
	}
	public void AddBotData(BotData data)
	{
		botSaveData.BotDatas.Add(data);
	}
	public List<BotData> GetBotDatas()
	{
		return botSaveData.BotDatas;
	}
	public BotData GetBotDataByName(string name)
	{
		foreach (BotData data in botSaveData.BotDatas)
		{
			if (data.BotName == name)
				return data;
		}

		Debug.LogError($"Data not found by name {name}");
		return null;
	}
	//public void AddSaveData(BotOld bot)
	//{
	//	BotData botData = new BotData();
	//	botData.BotName = bot.BotName;
	//	botData.BotPosition = bot.gameObject.transform.position;
	//	botData.message = bot.Message;
	//	AddBotData(botData);
	//}

    

}
