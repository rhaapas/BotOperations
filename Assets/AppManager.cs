using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BotEvent : UnityEvent<BotOld> { }

public class AppManager : NetworkBehaviour
{
	[SerializeField] private GameObject botPrefab;
	[SerializeField] private Transform botModeParent;
	[SerializeField] private SaveDataManager saveData;
	[SerializeField] private Transform newBotSpawnPoint;
	[SerializeField] private Transform botContainer;

	[SerializeField] public List<BotOld> botsInPlay = new List<BotOld>();
	[SerializeField] public BotOld selectedBot;


	private BotOld previousSelectedBot;

	public ModeSelection SelectedMode { get; private set; }
	public BotEvent BotSelectionChanged;

	private void Awake()
	{
		//singleton


		SelectedMode = ModeSelection.None;
		//if (IsHost || IsServer)
		//{
		//	StartCoroutine(Init());
		//}

	}

	public void Initialize()
	{
		Debug.Log("Initialize called..");
		botModeParent = this.transform.parent;
		if (IsHost || IsServer)
		{
			botModeParent = NetworkManager.LocalClient.PlayerObject.transform;
			Debug.Log($"botModeParent = {botModeParent.name}");
			StartCoroutine(InitHost());
		}
		else
		{
			Debug.Log("client...");
			StartCoroutine(InitClient());
		}
	}
	public IEnumerator InitClient()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("InitClient Called");
		botsInPlay = FindBots();
	}

	public IEnumerator InitHost()
	{
		Debug.Log("Initializing appManager");
		bool saveDataRetrieved = saveData.SaveDataRetrieved;
		while (!saveData.SaveDataRetrieved)
		{
			yield return null;
			saveDataRetrieved = saveData.SaveDataRetrieved;
		}
		Debug.Log(saveData.GetBotDatas().Count);
		foreach (BotData botData in saveData.GetBotDatas())
		{
			GameObject botObj = SpawnBot(botData.BotName, botData.BotPosition, Quaternion.identity, botContainer, botData.message);
			botObj.GetComponent<NetworkObject>().Spawn();
			BotOld bot = botObj.GetComponent<BotOld>();
			botsInPlay.Add(bot);
		}
	}
	private void Update()
	{
		if (selectedBot != previousSelectedBot)
			BotSelectionChanged?.Invoke(selectedBot);

		previousSelectedBot = selectedBot;
	}

	public void SpawnPlayerBot()
	{
		if (!IsInBotList(selectedBot.BotName))
		{
			BotData savedBot = saveData.GetBotDataByName(selectedBot.BotName);
			GameObject bot = SpawnBot(savedBot.BotName, savedBot.BotPosition, Quaternion.identity, botModeParent);
		}
		else
		{
			botModeParent.GetComponentInParent<NetworkObject>().transform.position = selectedBot.transform.position;
			selectedBot.transform.parent = botModeParent;
		}
		//selectedBot.ActivateCamera();
	}
	public void CreatePlayerBot(string name)
	{
		GameObject botObject = SpawnBot(name, newBotSpawnPoint.position, Quaternion.identity, botModeParent);
		BotOld bot = botObject.GetComponent<BotOld>();

		bot.Initialize(name, newBotSpawnPoint.position);

		botsInPlay.Add(bot);

		saveData.AddSaveData(bot);
		saveData.SaveBotData();


	}
	public GameObject SpawnBot(string name, Vector2 position, Quaternion rotation, Transform parent, string message = "")
	{
		GameObject botObject = Instantiate(botPrefab, position, rotation, parent);
		BotOld bot = botObject.GetComponent<BotOld>();
		bot.name = name;
		Debug.Log(message);
		bot.Initialize(name, position, message);
		return botObject;
	}
	private List<BotOld> FindBots()
	{
		Debug.Log("Getting bots..");
		//List<Bot> botList = FindObjectsOfType<Bot>().ToList();

		NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
		List<BotOld> botList = new List<BotOld>();
		Debug.Log($"NetworkObjects count: {networkObjects.Length}");
		foreach (NetworkObject no in networkObjects)
		{
			if (no.GetComponent<BotOld>() != null)
				botList.Add(no.GetComponent<BotOld>());
		}


		Debug.Log($"Bots found: {botList.Count}");
		//return FindObjectsOfType<Bot>().ToList();
		return botList;
	}
	public bool IsInBotList(string name)
	{
		foreach (BotOld bot in botsInPlay)
			if (bot.BotName == name)
				return true;

		return false;
	}

	public BotOld GetBotByName(string name)
	{
		foreach (BotOld bot in botsInPlay)
			if (bot.BotName == name)
				return bot;

		Debug.LogError("No bot found by that name");
		return null;
	}
	public void UpdateBotPosition(BotOld bot)
	{
		BotData botData = saveData.GetBotDataByName(bot.BotName);
		botData.BotPosition = bot.transform.position;
		saveData.SaveBotData();
	}
	public void UpdateBotMessage(BotOld bot)
	{
		BotData botData = saveData.GetBotDataByName(bot.BotName);
		botData.message = bot.Message;
		saveData.SaveBotData();
	}

	public void SetSelectedMode(ModeSelection modeSelection) { SelectedMode = modeSelection; }
	public void SetBotModeParent(Transform parent) { botModeParent = parent; }
}
