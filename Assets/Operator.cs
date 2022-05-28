using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Operator : MonoBehaviour
{
	[SerializeField] private TMP_InputField sendMessageInputField;
	[SerializeField] private Button sendMessageButton;
	[SerializeField] private AppManager appManager;
	[SerializeField] private TMP_Text selectedBotText;

	private void Awake()
	{
		appManager = FindObjectOfType<AppManager>();
		appManager.BotSelectionChanged.AddListener(SelectionChanged);
		sendMessageButton.onClick.AddListener(SendMessageToBot);
	}

	private void SelectionChanged(BotOld bot)
	{
		selectedBotText.text = bot.BotName;
	}

	public void SendMessageToBot()
	{
		string message = sendMessageInputField.text;
		Debug.Log("Sending message to bot: " + message);
		appManager.selectedBot.SetMessage(message);
	}

}
