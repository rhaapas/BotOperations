using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BotData
{
	public string BotName;
	public Vector2 BotPosition;
	public string message;
}
[System.Serializable]
public class BotSaveData
{
	public List<BotData> BotDatas = new List<BotData>();
}
