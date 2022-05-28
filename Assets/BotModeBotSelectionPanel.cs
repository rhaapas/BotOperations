using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotModeBotSelectionPanel : MonoBehaviour
{
	[SerializeField] private TMP_Text errorText;
	[SerializeField] private Button AddBotButton;
	[SerializeField] private TMP_InputField AddBotInputField;
	[SerializeField] private GameObject botDataButtonPrefab;
	[SerializeField] private AppManager appManager;
	[SerializeField] private GameObject scrollViewContainer;
	[SerializeField] private List<BotDataButton> botDataButtons = new List<BotDataButton>();

	private void Awake()
	{
		//gameObject.SetActive(false);
		AddBotButton.onClick.AddListener(AddBot);
		appManager = FindObjectOfType<AppManager>();
	}
	private void Start()
	{
		errorText.gameObject.SetActive(false);
	}

	public void SpawnButtons()
	{
		Debug.Log($"Instatiating buttons... appManager.botsInPlay size = {appManager.botsInPlay.Count}");
		foreach (BotOld bot in appManager.botsInPlay)
		{
			var dataButtonObject = Instantiate(botDataButtonPrefab, scrollViewContainer.transform);
			var dataButton = dataButtonObject.GetComponent<BotDataButton>();
			//dataButton.Initialize(bot.BotName);
			botDataButtons.Add(dataButton);
		}
	}

	public void AddBot()
	{
		string botName = AddBotInputField.text;

		if (botName == "" || botName.Length < 1)
		{
			errorText.text = "Name is too short.";
			errorText.gameObject.SetActive(true);
		}
		if (appManager.IsInBotList(botName))
		{
			errorText.text = "Bot with that name already found";
			errorText.gameObject.SetActive(true);
			return;
		}
		else
		{
			var botDataButton = Instantiate(botDataButtonPrefab, scrollViewContainer.transform);
			//botDataButton.GetComponent<BotDataButton>().Initialize(botName);

			if (errorText.gameObject.activeSelf == true)
				errorText.gameObject.SetActive(false);

			appManager.CreatePlayerBot(botName);

			gameObject.SetActive(false);

		}
	}



}
